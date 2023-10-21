#undef NOPE
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Fluxer.Net.Gateway;
using Fluxer.Net.Gateway.Data;
using Fluxer.Net.Objects.Data;
using Websocket.Client;
namespace Fluxer.Net;

public partial class GatewayClient
{
    #region Declares
    public string Token { get; set; }

    private readonly FluxerConfig _config;
    private WebsocketClient _gateway;
    private readonly Stopwatch _gatewayDuration = new();
    private int _sequence = 0;
    private bool _heartbeatStarted = false;
    private int _heartbeatInterval = 0;
    private DateTime _lastGatewayReEstablishAttempt = DateTime.Now;
    private string _sessionId = "";
    private Logger _logger;

    // build error from generated regex
    // temp. removed pending investigation.
    private static readonly Regex PacketSRegex = new(@"(?<=""s""\s*?:\s*?)\d*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    #endregion

    #region Meta
    public GatewayClient(string token, FluxerConfig config)
    {
        Token = token;
        _config = config;
        _logger = _config.Serilog ?? new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console().CreateLogger();
        _logger.Information("Initialized Fluxer.Net gateway client ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _config.Version);
        _logger.Verbose("Loaded with config {@Config}", _config);
    }
    #endregion

    #region Gateway
    public async Task ConnectAsync()
    {
        // disabled for testing
        // if (_gateway is not null && _gateway.State == WebSocketState.Open)
        //     await _gateway.CloseAsync();

        _gateway = new WebsocketClient(new(_config.FluxerGatewayUrl));
        _gateway.MessageReceived.Subscribe(x => GatewayMessageHandler(x.Text));
        _gateway.ReconnectionHappened.Subscribe(x => ReEstablishGatewayConnection(x));
        Stopwatch.StartNew();
        await _gateway.Start();

        var login = new GatewayPacket
        {
            OpCode = FluxerOpCode.Identify,
            Data = new IdentifyGatewayData(Token)
            {
                IgnoredGatewayEvents = _config.IgnoredGatewayEvents,
                Presence = _config.Presence
            }
        };

        SendGatewayPacket(login);
    }

    public void SendGatewayPacket<T>(T Data)
    {
        var text = JsonConvert.SerializeObject(Data, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        _logger.Debug("Sending serialized gateway packet {Data}", text);
        try
        {
            _gateway.Send(text);
        }
        catch
        {
            _logger.Warning("Failed to send gateway packet. Restarting the gateway. Some packets may be dropped.");
            ConnectAsync().GetAwaiter().GetResult();
        }
    }

    private void GatewayMessageHandler(string message)
    {
        // try deserializing the packet, fallback to regex
        _logger.Verbose("Received raw gateway message {Message}", message);
        try
        {
            var packet = JsonConvert.DeserializeObject<GatewayPacket>(message);
            _sequence = packet.Sequence ?? _sequence;
            _logger.Debug("Deserialized gateway packet {@Packet}", packet);
            switch (packet.OpCode)
            {
                case FluxerOpCode.Dispatch:
                    HandleDispatch(packet);
                    return;
                case FluxerOpCode.Heartbeat:
                case FluxerOpCode.Identify:
                case FluxerOpCode.PresenceUpdate:
                    throw new NotImplementedException();
                case FluxerOpCode.InvalidSession:
                    ConnectAsync().GetAwaiter().GetResult();
                    return;
                case FluxerOpCode.Reconnect:
                    ReEstablishGatewayConnection();
                    return;
                case FluxerOpCode.Hello:
                    HandleHello(packet);
                    return;
                case FluxerOpCode.HeartbeatAck:
                    HandleHeartbeatAck();
                    return;
            }
        }
        catch
        {
            var result = PacketSRegex.Match(message);
            _sequence = Convert.ToInt32(result.Value);
            _logger.Warning("Failed to parse a gateway event. This can happen when the OpCode is unsupported or a dispatch failed to parse.");
        }

    }

    // old closed handler, kept in case it's needed someday 
#if NOPE
    private void GatewayClosedHandler(object? sender, EventArgs e)
    {
        if (_deferDisconnect)
        {
            _deferDisconnect = false;
            return;
        }

        if (e is ClosedEventArgs nE)
        {
            _logger.Information("Websocket closed with code {Code}:{Reason}. It should auto restart.", nE.Code, nE.Reason ?? "Unknown");
        }
        else
        {
            _logger.Information("Websocket closed. It should auto restart. ClosedEventArgs was null.");
        }

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
#endif
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
            // I miss spaces, man, it was more whimsical
            case "CHANNEL_CREATE":
                ChannelCreate?.Invoke(p.Data as ChannelGatewayData);
                return;
            case "CHANNEL_UPDATE":
                ChannelUpdate?.Invoke(p.Data as ChannelGatewayData);
                return;
            case "CHANNEL_DELETE":
                ChannelDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            case "USER_UPDATE":
                UserUpdate?.Invoke(p.Data as UserGatewayData);
                return;
            case "COMMUNITY_MEMBER_CREATE":
                CommunityMemberCreate?.Invoke(p.Data as CommunityMemberGatewayData);
                return;
            case "COMMUNITY_MEMBER_UPDATE":
                CommunityMemberUpdate?.Invoke(p.Data as CommunityMemberGatewayData);
                return;
            case "COMMUNITY_MEMBER_DELETE":
                CommunityMemberDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            case "PRESENCE_UPDATE":
                PresenceUpdate?.Invoke(p.Data as PresenceGatewayData);
                return;
            case "COMMUNITY_CREATE":
                CommunityCreate?.Invoke(p.Data as CommunityGatewayData);
                return;
            case "COMMUNITY_UPDATE":
                CommunityUpdate?.Invoke(p.Data as CommunityGatewayData);
                return;
            case "COMMUNITY_DELETE":
                CommunityDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            case "TYPING_START":
                TypingStart?.Invoke(p.Data as TypingGatewayData);
                return;
            case "TYPING_STOP":
                TypingStop?.Invoke(p.Data as TypingGatewayData);
                return;
            case "COMMUNITY_ROLE_CREATE":
                RoleCreate?.Invoke(p.Data as RoleGatewayData);
                return;
            case "COMMUNITY_ROLE_UPDATE":
                RoleUpdate?.Invoke(p.Data as RoleGatewayData);
                return;
            case "COMMUNITY_ROLE_DELETE":
                RoleDelete?.Invoke(p.Data as EntityRemovedGatewayData);
                return;
            default:
                _logger.Warning("Unhandled dispatch {Dispatch}", p.Dispatch);
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

    private void ReEstablishGatewayConnection(ReconnectionInfo? info = null)
    {
        if (info is not null)
            if (info.Type is ReconnectionType.Error or ReconnectionType.ByServer)
                Log.Error("Reconnected with info {info}", info);
            else
                Log.Information("Reconnected with info {info}", info);
        else
            Log.Warning("Reconnected without connection info");

        if (_lastGatewayReEstablishAttempt.AddSeconds(_config.ReconnectAttemptDelay) > DateTime.Now)
        {
            _logger.Warning("Cannot reestablish more than once every {Timeout} seconds. Blocking until the time expires.", _config.ReconnectAttemptDelay);
            Task.Delay(_config.ReconnectAttemptDelay * 1000).GetAwaiter().GetResult();
        }

        _lastGatewayReEstablishAttempt = DateTime.Now;
        _gatewayDuration.Restart();

        _logger.Information("Attempting to reestablish the gateway connection after {time}ms", _gatewayDuration.ElapsedMilliseconds);

        var packet = new GatewayPacket()
        {
            OpCode = FluxerOpCode.Resume,
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
                OpCode = FluxerOpCode.Heartbeat,
            };
            SendGatewayPacket(packet);
        }
    }

    public void SetStatus(Status status)
    {
        var packet = new GatewayPacket()
        {
            Data = new PresenceUpdateGatewayData(status),
            OpCode = FluxerOpCode.PresenceUpdate
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

    public delegate void ChannelCreateEvent(ChannelGatewayData data);
    public event ChannelCreateEvent ChannelCreate;

    public delegate void ChannelUpdateEvent(ChannelGatewayData data);
    public event ChannelUpdateEvent ChannelUpdate;

    public delegate void ChannelDeleteEvent(EntityRemovedGatewayData data);
    public event ChannelDeleteEvent ChannelDelete;

    // user

    public delegate void UserUpdateEvent(UserGatewayData data);
    public event UserUpdateEvent UserUpdate;

    // presence
    public delegate void PresenceUpdateEvent(PresenceGatewayData data);
    public event PresenceUpdateEvent PresenceUpdate;

    // community
    public delegate void CommunityCreateEvent(CommunityGatewayData data);
    public event CommunityCreateEvent CommunityCreate;

    public delegate void CommunityUpdateEvent(CommunityGatewayData data);
    public event CommunityUpdateEvent CommunityUpdate;

    public delegate void CommunityDeleteEvent(EntityRemovedGatewayData data);
    public event CommunityDeleteEvent CommunityDelete;

    // typing
    public delegate void TypingStartEvent(TypingGatewayData data);
    public event TypingStartEvent TypingStart;

    public delegate void TypingStopEvent(TypingGatewayData data);
    public event TypingStopEvent TypingStop;

    // community member

    public delegate void CommunityMemberCreateEvent(CommunityMemberGatewayData data);
    public event CommunityMemberCreateEvent CommunityMemberCreate;

    public delegate void CommunityMemberUpdateEvent(CommunityMemberGatewayData data);
    public event CommunityMemberUpdateEvent CommunityMemberUpdate;

    public delegate void CommunityMemberDeleteEvent(EntityRemovedGatewayData data);
    public event CommunityMemberDeleteEvent CommunityMemberDelete;

    // role

    public delegate void RoleCreateEvent(RoleGatewayData data);
    public event RoleCreateEvent RoleCreate;

    public delegate void RoleUpdateEvent(RoleGatewayData data);
    public event RoleUpdateEvent RoleUpdate;

    public delegate void RoleDeleteEvent(EntityRemovedGatewayData data);
    public event RoleDeleteEvent RoleDelete;

    #endregion

    #endregion
}
