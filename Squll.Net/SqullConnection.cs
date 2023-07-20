// using System.Net.WebSockets;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Squll.Net.Extensions;
using Squll.Net.Gateway;
using Squll.Net.Objects;
using WebSocket4Net;

namespace Squll.Net;

public class SqullConnection
{
    public string Token { get; set; }
    private WebSocket ws;
    public HttpClient HttpClient = new();
    public int Sequence = 0;
    private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented
    };

    public SqullConnection(string token)
    {
        Token = token;
        HttpClient.DefaultRequestHeaders.Add("Authorization", Token);
    }

    public async Task ConnectToGateway()
    {
        ws = new WebSocket("wss://gateway.squll.com?v=1&encoding=json");
        ws.MessageReceived += HandleMessage;
        await ws.OpenAsync();
    }

    private void HandleMessage(object? sender, MessageReceivedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"⬅ {e.Message}");
        var message = JsonConvert.DeserializeObject<GatewayPacket>(e.Message, jsonSettings);
        if (message.Sequence != null)
            Sequence = (int)message.Sequence;

        switch (message.OpCode)
        {
            case SqullOpCode.Dispatch:
                HandleDispatch(message);
                break;
            case SqullOpCode.Hello:
                HandleHello(message);
                break;
        }
    }

    private void HandleDispatch(GatewayPacket packet)
    {
        Console.WriteLine(packet);
    }

    private void HandleHello(GatewayPacket packet)
    {
        var login = new GatewayPacket
        {
            OpCode = SqullOpCode.Identify,
            Data = new IdentifyGatewayData(Token)
        };

        var data = packet.Data as HelloGatewayData;
        _ = Heartbeat(data.HeartbeatInterval);

        var content = JsonConvert.SerializeObject(login);
        SendMessage(content);

    }

    private void SendMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"➡ {message}");
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
            throw new SqullException("Squll returned non-success code")
            {
                SqullData = content
            };
        return JsonConvert.DeserializeObject<SquadProperties>(content);
    }

    public async Task<SquadProperties> GetSquad(ulong id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.squll.com/v1/squads/{id}");
        var response = await HttpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new SqullException("Squll returned non-success code")
            {
                SqullData = content
            };
        return JsonConvert.DeserializeObject<SquadProperties>(content);
    }

    private async Task Heartbeat(int interval)
    {
        while (true)
        {
            var packet = new UntypedDataGatewayPacket()
            {
                Data = Sequence,
                OpCode = SqullOpCode.Heartbeat,
            };
            SendMessage(JsonConvert.SerializeObject(packet));
            await Task.Delay(interval);
        }
    }


}
