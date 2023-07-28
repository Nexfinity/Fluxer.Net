// using System.Net.WebSockets;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Squll.Net.Extensions;
using Squll.Net.Gateway;
using Squll.Net.Objects;
using WebSocket4Net;

namespace Squll.Net;

public partial class SqullConnection
{
    [GeneratedRegex(@"(?<=""s""\s*?:\s*?)\d*", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex PacketSRegex();

    public string Token { get; set; }
    private WebSocket ws;
    public HttpClient HttpClient = new();
    public int Sequence = 0;
    Stopwatch stopwatch;
    private readonly JsonSerializerSettings jsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented
    };
    private Task heart = null;

    public SqullConnection(string token)
    {
        Token = token;
        HttpClient.DefaultRequestHeaders.Add("Authorization", Token);
        // _ = HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
    }

    public async Task ConnectToGateway()
    {
        ws = new WebSocket("wss://gateway.squll.com?v=1&encoding=json");
        ws.MessageReceived += HandleMessage;
        ws.Closed += HandleClosed;
        ws.EnableAutoSendPing = false;
        ws.NoDelay = true;
        stopwatch = new();
        stopwatch.Start();
        await ws.OpenAsync();
    }

    private void HandleClosed(object? sender, EventArgs e)
    {
        stopwatch.Stop();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine($"Disconnected from websocket after {stopwatch.ElapsedMilliseconds}ms. Attempting to reconnect.");
        stopwatch = new();
        stopwatch.Start();
        ws.Open();
    }

    private void HandleMessage(object? sender, MessageReceivedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"⬅ {e.Message.Replace(Token, "[[ TOKEN REDACTED ]]")}");
        var message = new GatewayPacket();
        try
        {
            message = JsonConvert.DeserializeObject<GatewayPacket>(e.Message, jsonSettings);
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Failed to deserialize packet. This can happen when the opcode is opcode is unknown or the data is unsupported.");
            // extract packet sequence with regex because we need it to heartbeat
            var match = PacketSRegex().Match(e.Message);
            Sequence = Convert.ToInt32(match.Value);
            return;
        }
        if (message.Sequence != null)
            Sequence = (int)message.Sequence;

        switch (message.OpCode)
        {
            case SqullOpCode.Dispatch:
                _ = Task.Run(() => HandleDispatch(message));
                break;
            case SqullOpCode.Hello:
                HandleHello(message);
                break;
        }
    }

    private void HandleDispatch(GatewayPacket packet)
    {
        switch (packet.Dispatch)
        {
            case "READY":
                Ready?.Invoke(packet.Data as ReadyGatewayData);
                break;
            case "MESSAGE_CREATE":
                MessageCreated?.Invoke(packet.Data as MessageGatewayData);
                break;
            case "MESSAGE_UPDATE":
                MessageUpdated?.Invoke(packet.Data as MessageGatewayData);
                break;
        }
    }

    private void HandleHello(GatewayPacket packet)
    {
        var login = new GatewayPacket
        {
            OpCode = SqullOpCode.Identify,
            Data = new IdentifyGatewayData(Token)
        };

        var data = packet.Data as HelloGatewayData;

        // avoid multiple heartbeat threads
        heart ??= Heartbeat(data.HeartbeatInterval);

        var content = JsonConvert.SerializeObject(login);
        SendMessage(content);

    }

    private void SendMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"➡ {message.Replace(Token, "[[ TOKEN REDACTED ]]")}");
        ws.Send(message);
    }

    public void SetStatus(string status)
    {
        var packet = new GatewayPacket()
        {
            OpCode = SqullOpCode.PresenceUpdate,
            Data = new PresenceUpdateGatewayData(status)
        };
        SendMessage(JsonConvert.SerializeObject(packet));
    }

    public async Task<SquadProperties> JoinSquad(string invite)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.squll.com/v1/invites/{invite}");
        var response = await HttpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new SqullApiException("Squll returned non-success code", content);
        return JsonConvert.DeserializeObject<SquadProperties>(content);
    }

    public async Task<SquadProperties> GetSquad(ulong id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.squll.com/v1/squads/{id}");
        var response = await HttpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new SqullApiException("Squll returned non-success code", content);
        return JsonConvert.DeserializeObject<SquadProperties>(content);
    }

    // TODO: better invalid session detection and resuming.
    //       "this is fine" for now.
    private async Task Heartbeat(int interval)
    {
        var jitter = Random.Shared.Next(1);
        while (true)
        {
            await Task.Delay(interval + jitter);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(Sequence);
            var packet = new HeartbeatPacketOfDoom()
            {
                Data = Sequence,
                OpCode = SqullOpCode.Heartbeat,
            };
            SendMessage(JsonConvert.SerializeObject(packet));
        }
    }

    public async Task<Message> SendMessage(ulong spaceId, Message message)
    {
        var content = JsonConvert.SerializeObject(message, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        var result = await SendDataToSqull(HttpMethod.Post, $"spaces/{spaceId}/messages", content);
        return JsonConvert.DeserializeObject<Message>(result);
    }

    public async Task<string> SendDataToSqull(HttpMethod method, string route, string? content = null, int apiVersion = 1)
    {
        var reqMessage = new HttpRequestMessage(method, $"https://api.squll.com/v{apiVersion}/{route}");
        if (content is not null)
            reqMessage.Content = new StringContent(content!, mediaType: new("application/json"));

        var result = await HttpClient.SendAsync(reqMessage);

        if (!result.IsSuccessStatusCode)
            throw new SqullApiException("Squll returned error on message send", content);
        return await result.Content.ReadAsStringAsync();
    }

    public delegate void ReadyEvent(ReadyGatewayData data);
    public event ReadyEvent Ready;
    public delegate void MessageCreatedEvent(MessageGatewayData data);
    public event MessageCreatedEvent MessageCreated;
    public delegate void MessageUpdatedEvent(MessageGatewayData data);
    public event MessageUpdatedEvent MessageUpdated;
}
