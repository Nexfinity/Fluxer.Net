// using System.Net.WebSockets;
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

    public SqullConnection(string token)
    {
        Token = token;
        HttpClient.DefaultRequestHeaders.Add("Authorization", Token);
    }

    public async Task ConnectToGateway()
    {
        var login = new LoginGatewayPacket(new(Token));
        var content = JsonConvert.SerializeObject(login);

        ws = new WebSocket("wss://gateway.squll.com?v=1&encoding=json");
        ws.MessageReceived += HandleMessage;
        await ws.OpenAsync();
        SendMessage(content);
        // var uri = new Uri("ws://gateway.squll.com?v=1&encoding=json");
        // using var handler = new SocketsHttpHandler();
        // using var ws = new ClientWebSocket();
        // await ws.ConnectAsync(uri, CancellationToken.None);
        // await ws.SendAsync()
    }

    private void HandleMessage(object? sender, MessageReceivedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"⬅ {e.Message}");
        var message = JsonConvert.DeserializeObject<IGatewayPacket>(e.Message);
        if (message.Sequence != null)
            Sequence = (int)message.Sequence;

        switch (message.OpCode)
        {
            case SqullOpCode.Ready:
                HandleReady(message as ReadyGatewayPacket);
                break;
        }
    }

    private void HandleReady(ReadyGatewayPacket packet)
    {
        Task.Run(() =>
        {

        })
    }

    private void SendMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"➡ {message}");
        ws.Send(message);
    }

    public void SetStatus(string status)
    {
        var packet = new StatusUpdateGatewayPacket(new(status));
        SendMessage(JsonConvert.SerializeObject(packet));
    }

    public async Task<Squad> JoinSquad(string invite)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.squll.com/v1/invites/{invite}");
        var response = await HttpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new SqullException("Squll returned non-success code")
            {
                SqullData = content
            };
        return JsonConvert.DeserializeObject<Squad>(content);
    }

    public async Task<Squad> GetSquad(ulong id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.squll.com/v1/squads/{id}");
        var response = await HttpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new SqullException("Squll returned non-success code")
            {
                SqullData = content
            };
        return JsonConvert.DeserializeObject<Squad>(content);
    }


}
