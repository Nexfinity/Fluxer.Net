using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Squll.Net.Extensions;
using Squll.Net.Gateway;
using Squll.Net.Objects;
using WebSocket4Net;
namespace Squll.Net;

public partial class SqullConnection
{
    #region Declares
    public string Token { get; set; }
    public HttpClient HttpClient { get; set; }

    private readonly SqullConfig _config;
    private WebSocket _gateway;
    private readonly Stopwatch _gatewayDuration = new();
    private int _sequence = 0;
    private bool _heartbeatStarted = false;
    private int _heartbeatInterval = 0;
    private DateTime _lastGatewayReEstablishAttempt = DateTime.Now;
    private string _sessionId = "";
    private bool _deferDisconnect = false;

    [GeneratedRegex(@"(?<=""s""\s*?:\s*?)\d*", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex PacketSRegex();
    #endregion

    #region Meta
    public SqullConnection(string token, SqullConfig config)
    {
        Token = token;
        _config = config;
        HttpClient = _config.HttpClient ?? new();
        Log.Logger = (_config.SerilogConfig
            ?? new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()).CreateLogger();
        Log.Information("Initialized Squll.Net ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _config.Version);
        Log.Verbose("Loaded with config {@Config}", _config);
    }

    public async Task<TResponse> MakeSqullApiRequest<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false)
    {
        Log.Verbose("Sending {@Data} to {Route}", data, route);
        var req = new HttpRequestMessage()
        {
            Method = method,
            Content = new StringContent(JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }), new MediaTypeHeaderValue("application/json")),
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new SqullApiException($"Squll returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    public async Task<TResponse> MakeSqullApiRequest<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new SqullApiException($"Squll returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    public async Task<HttpStatusCode> MakeSqullApiRequest(HttpMethod method, string route, bool throwOnNonSuccess = false)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route} with response code {Code}", method, route, result.StatusCode);
        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new SqullApiException($"Squll returned a non-success code {result.StatusCode}", await result.Content.ReadAsStringAsync());

        return result.StatusCode;
    }
    #endregion

    #region API
    public async Task<Message> SendMessage(ulong spaceId, Message message)
        => await MakeSqullApiRequest<Message, Message>(HttpMethod.Post, $"spaces/{spaceId}/messages", message, true);

    public async Task<SquadProperties> JoinSquad(string invite)
        => await MakeSqullApiRequest<SquadProperties>(HttpMethod.Post, $"invites/{invite}", true);

    public async Task LeaveSquad(ulong Id)
        => await MakeSqullApiRequest(HttpMethod.Delete, $"users/@me/squads/{Id}", true);

    public async Task<SquadProperties> GetSquad(ulong squadId)
        => await MakeSqullApiRequest<SquadProperties>(HttpMethod.Get, $"squads/{squadId}", true);


    #endregion

    #region Gateway
    public async Task ConnectAsync()
    {
        if (_gateway is not null && _gateway.State == WebSocketState.Open)
            await _gateway.CloseAsync();

        _gateway = new WebSocket(_config.SqullGatewayUrl)
        {
            EnableAutoSendPing = false,
            // NoDelay = true,
        };
        _gateway.Closed += GatewayClosedHandler;
        _gateway.MessageReceived += GatewayMessageHandler;
        Stopwatch.StartNew();
        await _gateway.OpenAsync();

        var login = new GatewayPacket
        {
            OpCode = SqullOpCode.Identify,
            Data = new IdentifyGatewayData(Token)
        };

        SendGatewayPacket(login);
    }

    public void SendGatewayPacket<T>(T Data)
    {
        var text = JsonConvert.SerializeObject(Data, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        Log.Debug("Sending serialized gateway packet {Data}", text);
        _gateway.Send(text);
    }

    private void GatewayMessageHandler(object? sender, MessageReceivedEventArgs e)
    {
        // try deserializing the packet, fallback to regex
        Log.Verbose("Received raw gateway message {Message}", e.Message);
        try
        {
            var packet = JsonConvert.DeserializeObject<GatewayPacket>(e.Message);
            _sequence = packet.Sequence ?? _sequence;
            Log.Debug("Deserialized gateway packet {@Packet}", packet);
            switch (packet.OpCode)
            {
                case SqullOpCode.Dispatch:
                    HandleDispatch(packet);
                    return;
                case SqullOpCode.Heartbeat:
                case SqullOpCode.Identify:
                case SqullOpCode.PresenceUpdate:
                    throw new NotImplementedException();
                case SqullOpCode.InvalidSession:
                    ConnectAsync().GetAwaiter().GetResult();
                    return;
                case SqullOpCode.Reconnect:
                    ReEstablishGatewayConnection();
                    return;
                case SqullOpCode.Hello:
                    HandleHello(packet);
                    return;
                case SqullOpCode.HeartbeatAck:
                    HandleHeartbeatAck();
                    return;
            }
        }
        catch
        {
            var result = PacketSRegex().Match(e.Message);
            _sequence = Convert.ToInt32(result.Value);
            Log.Warning("Failed to parse a gateway event. This can happen when the OpCode is unsupported or a dispatch failed to parse.");
        }

    }

    private void GatewayClosedHandler(object? sender, EventArgs e)
    {
        if (_deferDisconnect)
        {
            _deferDisconnect = false;
            return;
        }

        var nE = e as ClosedEventArgs;
        Log.Information("Websocket closed with code {Code}:{Reason}. It should auto restart.", nE.Code, nE.Reason);
        if (_gateway.State != WebSocketState.Closed)
        {
            _deferDisconnect = true;
            _gateway.CloseAsync().GetAwaiter().GetResult();
        }
        try
        {
            _gateway.OpenAsync().GetAwaiter().GetResult();
            ReEstablishGatewayConnection();
        }
        catch
        {
            ConnectAsync().GetAwaiter().GetResult();
        }
    }

    private void HandleDispatch(GatewayPacket p)
    {
        switch (p.Dispatch)
        {
            case "READY":
                var data = p.Data as ReadyGatewayData;
                _sessionId = data.SessionId;
                Ready?.Invoke(data);
                return;
            case "RESUMED":
                Resumed?.Invoke();
                return;
            case "MESSAGE_CREATE":
                MessageCreate?.Invoke(p.Data as MessageGatewayData);
                return;
            case "MESSAGE_UPDATE":
                MessageUpdate?.Invoke(p.Data as MessageGatewayData);
                return;
            case "MESSAGE_DELETE":
                MessageDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            case "SPACE_CREATE":
                SpaceCreate?.Invoke(p.Data as SpaceGatewayData);
                return;
            case "SPACE_UPDATE":
                SpaceUpdate?.Invoke(p.Data as SpaceGatewayData);
                return;
            case "SPACE_DELETE":
                SpaceDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            case "USER_UPDATE":
                UserUpdate?.Invoke(p.Data as UserGatewayData);
                return;
            case "SQUAD_MEMBER_UPDATE":
                SquadMemberUpdate?.Invoke(p.Data as SquadMemberGatewayData);
                return;
            case "SQUAD_MEMBER_DELETE":
                SquadMemberDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            case "PRESENCE_UPDATE":
				PresenceUpdate?.Invoke(p.Data as PresenceGatewayData);
				return;
            default:
	            Log.Warning("Unhandled dispatch {Dispatch}", p.Dispatch);
	            break;
        }
    }

    private void HandleHello(GatewayPacket packet)
    {

        var data = packet.Data as HelloGatewayData;

        // avoid multiple heartbeat threads
        _heartbeatInterval = data.HeartbeatInterval;
        if (!_heartbeatStarted)
            Task.Run(() => HandleHeartbeat());
        _heartbeatStarted = true;
    }

    private void ReEstablishGatewayConnection()
    {
        if (_lastGatewayReEstablishAttempt.AddSeconds(_config.ReconnectAttemptDelay) > DateTime.Now)
        {
            Log.Warning("Cannot reestablish more than once every {Timeout} seconds. Blocking until the time expires.", _config.ReconnectAttemptDelay);
            Task.Delay(_config.ReconnectAttemptDelay * 1000).GetAwaiter().GetResult();
        }

        _lastGatewayReEstablishAttempt = DateTime.Now;
        _gatewayDuration.Restart();

        Log.Information("Attempting to reestablish the gateway connection after {time}ms", _gatewayDuration.ElapsedMilliseconds);

        var packet = new GatewayPacket()
        {
            OpCode = SqullOpCode.Resume,
            Data = new ReconnectGatewayData()
            {
                Sequence = _sequence,
                SessionId = _sessionId,
                Token = Token
            }
        };
        SendGatewayPacket(packet);
    }

    private void HandleHeartbeatAck()
    {
        HeartbeatAck?.Invoke();
    }

    private async Task HandleHeartbeat()
    {
        var jitter = Random.Shared.Next(1);
        while (true)
        {
            await Task.Delay(_heartbeatInterval + jitter);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(_sequence);
            var packet = new HeartbeatPacketOfDoom()
            {
                Data = _sequence,
                OpCode = SqullOpCode.Heartbeat,
            };
            SendGatewayPacket(packet);
        }
    }

    public void SetStatus(string status)
    {
        var packet = new GatewayPacket()
        {
            Data = new PresenceUpdateGatewayData(status),
            OpCode = SqullOpCode.PresenceUpdate
        };
        SendGatewayPacket(packet);
    }

    #region events
    // non-dispatch events

    // generic

    public delegate void HeartbeatAckEvent();
    public event HeartbeatAckEvent HeartbeatAck;

    public delegate void ReadyEvent(ReadyGatewayData data);
    public event ReadyEvent Ready;

    public delegate void ResumedEvent();
    public event ResumedEvent Resumed;

    // message

    public delegate void MessageCreateEvent(MessageGatewayData data);
    public event MessageCreateEvent MessageCreate;

    public delegate void MessageUpdateEvent(MessageGatewayData data);
    public event MessageCreateEvent MessageUpdate;

    public delegate void MessageDeleteEvent(EntityRemovedGatewayData data);
    public event MessageDeleteEvent MessageDelete;

    // space

    public delegate void SpaceCreateEvent(SpaceGatewayData data);
    public event SpaceCreateEvent SpaceCreate;

    public delegate void SpaceUpdateEvent(SpaceGatewayData data);
    public event SpaceUpdateEvent SpaceUpdate;

    public delegate void SpaceDeleteEvent(EntityRemovedGatewayData data);
    public event SpaceDeleteEvent SpaceDelete;

    // user

    public delegate void UserUpdateEvent(UserGatewayData data);
    public event UserUpdateEvent UserUpdate;
    
    // presence
    public delegate void PresenceUpdateEvent(PresenceGatewayData data);
    public event PresenceUpdateEvent PresenceUpdate;

    // squad member

    public delegate void SquadMemberCreateEvent(SquadMemberGatewayData data);
    public event SquadMemberCreateEvent SquadMemberCreate;

    public delegate void SquadMemberUpdateEvent(SquadMemberGatewayData data);
    public event SquadMemberUpdateEvent SquadMemberUpdate;

    public delegate void SquadMemberDeleteEvent(EntityRemovedGatewayData data);
    public event SquadMemberDeleteEvent SquadMemberDelete;

    #endregion

    #endregion
}
