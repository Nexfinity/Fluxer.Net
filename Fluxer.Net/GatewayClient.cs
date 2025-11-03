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

public partial class GatewayClient : IDisposable
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
    private CancellationTokenSource? _heartbeatCancellation;
    private readonly SemaphoreSlim _reconnectLock = new(1, 1);
    private bool _disposed = false;

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
        _logger.Debug("Sending serialized gateway packet {Enums}", text);
        try
        {
            _gateway.Send(text);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to send gateway packet. Scheduling reconnection. Some packets may be dropped.");
            // Don't block - schedule reconnection on thread pool
            _ = Task.Run(async () =>
            {
                try
                {
                    await ConnectAsync();
                }
                catch (Exception reconnectEx)
                {
                    _logger.Error(reconnectEx, "Failed to reconnect after send failure");
                }
            });
        }
    }

    private void GatewayMessageHandler(string message)
    {
        // try deserializing the packet, fallback to regex
        _logger.Verbose("Received raw gateway message {Message}", message);
        try
        {
            var packet = JsonConvert.DeserializeObject<GatewayPacket>(message);
            if (packet == null)
            {
                _logger.Warning("Deserialized gateway packet was null");
                return;
            }

            _sequence = packet.Sequence ?? _sequence;
            _logger.Debug("Deserialized gateway packet {@Packet}", packet);
            switch (packet.OpCode)
            {
                case FluxerOpCode.Dispatch:
                    HandleDispatch(packet);
                    return;
                case FluxerOpCode.Heartbeat:
                    _logger.Debug("Received Heartbeat opcode from server");
                    return;
                case FluxerOpCode.Identify:
                    _logger.Debug("Received Identify opcode from server (unexpected)");
                    return;
                case FluxerOpCode.PresenceUpdate:
                    _logger.Debug("Received PresenceUpdate opcode from server (unexpected)");
                    return;
                case FluxerOpCode.VoiceStateUpdate:
                    _logger.Debug("Received VoiceStateUpdate opcode from server");
                    return;
                case FluxerOpCode.VoiceServerPing:
                    _logger.Debug("Received VoiceServerPing opcode from server");
                    return;
                case FluxerOpCode.RequestGuildMembers:
                    _logger.Debug("Received RequestGuildMembers opcode from server (unexpected)");
                    return;
                case FluxerOpCode.CallConnect:
                    _logger.Debug("Received CallConnect opcode from server");
                    return;
                case FluxerOpCode.GuildSubscriptions:
                    _logger.Debug("Received GuildSubscriptions opcode from server");
                    return;
                case FluxerOpCode.InvalidSession:
                    _logger.Warning("Received InvalidSession opcode, reconnecting");
                    // Don't block the message handler - reconnect asynchronously
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ConnectAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Failed to reconnect after invalid session");
                        }
                    });
                    return;
                case FluxerOpCode.Reconnect:
                    _logger.Warning("Received Reconnect opcode from server");
                    // Don't block the message handler - reconnect asynchronously
                    _ = Task.Run(async () => await ReEstablishGatewayConnectionAsync());
                    return;
                case FluxerOpCode.Hello:
                    HandleHello(packet);
                    return;
                case FluxerOpCode.HeartbeatAck:
                    HandleHeartbeatAck();
                    return;
                default:
                    _logger.Warning("Received unknown OpCode: {OpCode}", packet.OpCode);
                    return;
            }
        }
        catch (JsonException ex)
        {
            _logger.Warning(ex, "Failed to deserialize gateway packet. Attempting to extract sequence from raw message.");
            try
            {
                var result = PacketSRegex.Match(message);
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    _sequence = Convert.ToInt32(result.Value);
                    _logger.Debug("Extracted sequence {Sequence} from malformed packet", _sequence);
                }
                else
                {
                    _logger.Warning("Could not extract sequence from malformed packet");
                }
            }
            catch (Exception regexEx)
            {
                _logger.Error(regexEx, "Failed to extract sequence using regex fallback");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error in gateway message handler");
        }
    }

    private void HandleDispatch(GatewayPacket p)
    {
        switch (p.Dispatch)
        {
            case "READY":
                if (p.Data is ReadyGatewayData readyData)
                {
                    _sessionId = readyData.SessionId;
                    Ready?.Invoke(readyData);
                }
                else
                {
                    _logger.Warning("READY event received but data could not be cast to ReadyGatewayData");
                }
                return;
            case "RESUMED":
                Resumed?.Invoke();
                return;
            case "MESSAGE_CREATE":
                if (p.Data is MessageGatewayData messageCreateData)
                    MessageCreate?.Invoke(messageCreateData);
                else
                    _logger.Warning("MESSAGE_CREATE event received but data could not be cast to MessageGatewayData");
                return;
            case "MESSAGE_UPDATE":
                if (p.Data is MessageGatewayData messageUpdateData)
                    MessageUpdate?.Invoke(messageUpdateData);
                else
                    _logger.Warning("MESSAGE_UPDATE event received but data could not be cast to MessageGatewayData");
                return;
            case "MESSAGE_DELETE":
                if (p.Data is EntityRemovedGatewayData messageDeleteData)
                    MessageDelete?.Invoke(messageDeleteData);
                else
                    _logger.Warning("MESSAGE_DELETE event received but data could not be cast to EntityRemovedGatewayData");
                return;
            case "CHANNEL_CREATE":
                if (p.Data is ChannelGatewayData channelCreateData)
                    ChannelCreate?.Invoke(channelCreateData);
                else
                    _logger.Warning("CHANNEL_CREATE event received but data could not be cast to ChannelGatewayData");
                return;
            case "CHANNEL_UPDATE":
                if (p.Data is ChannelGatewayData channelUpdateData)
                    ChannelUpdate?.Invoke(channelUpdateData);
                else
                    _logger.Warning("CHANNEL_UPDATE event received but data could not be cast to ChannelGatewayData");
                return;
            case "CHANNEL_DELETE":
                if (p.Data is EntityRemovedGatewayData channelDeleteData)
                    ChannelDelete?.Invoke(channelDeleteData);
                else
                    _logger.Warning("CHANNEL_DELETE event received but data could not be cast to EntityRemovedGatewayData");
                return;
            case "USER_UPDATE":
                if (p.Data is UserGatewayData userUpdateData)
                    UserUpdate?.Invoke(userUpdateData);
                else
                    _logger.Warning("USER_UPDATE event received but data could not be cast to UserGatewayData");
                return;
            case "PRESENCE_UPDATE":
                if (p.Data is PresenceGatewayData presenceData)
                    PresenceUpdate?.Invoke(presenceData);
                else
                    _logger.Warning("PRESENCE_UPDATE event received but data could not be cast to PresenceGatewayData");
                return;
            case "TYPING_START":
                if (p.Data is TypingGatewayData typingStartData)
                    TypingStart?.Invoke(typingStartData);
                else
                    _logger.Warning("TYPING_START event received but data could not be cast to TypingGatewayData");
                return;
            case "TYPING_STOP":
                if (p.Data is TypingGatewayData typingStopData)
                    TypingStop?.Invoke(typingStopData);
                else
                    _logger.Warning("TYPING_STOP event received but data could not be cast to TypingGatewayData");
                return;

            // Message reactions
            case "MESSAGE_REACTION_ADD":
                if (p.Data is MessageReactionGatewayData reactionAddData)
                    MessageReactionAdd?.Invoke(reactionAddData);
                else
                    _logger.Warning("MESSAGE_REACTION_ADD event received but data could not be cast to MessageReactionGatewayData");
                return;
            case "MESSAGE_REACTION_REMOVE":
                if (p.Data is MessageReactionGatewayData reactionRemoveData)
                    MessageReactionRemove?.Invoke(reactionRemoveData);
                else
                    _logger.Warning("MESSAGE_REACTION_REMOVE event received but data could not be cast to MessageReactionGatewayData");
                return;
            case "MESSAGE_REACTION_REMOVE_ALL":
                if (p.Data is EntityRemovedGatewayData reactionRemoveAllData)
                    MessageReactionRemoveAll?.Invoke(reactionRemoveAllData);
                else
                    _logger.Warning("MESSAGE_REACTION_REMOVE_ALL event received but data could not be cast to EntityRemovedGatewayData");
                return;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                if (p.Data is MessageReactionRemoveEmojiGatewayData reactionRemoveEmojiData)
                    MessageReactionRemoveEmoji?.Invoke(reactionRemoveEmojiData);
                else
                    _logger.Warning("MESSAGE_REACTION_REMOVE_EMOJI event received but data could not be cast to MessageReactionRemoveEmojiGatewayData");
                return;

            // Message bulk operations
            case "MESSAGE_DELETE_BULK":
                if (p.Data is MessageBulkDeleteGatewayData bulkDeleteData)
                    MessageDeleteBulk?.Invoke(bulkDeleteData);
                else
                    _logger.Warning("MESSAGE_DELETE_BULK event received but data could not be cast to MessageBulkDeleteGatewayData");
                return;
            case "MESSAGE_ACK":
                if (p.Data is MessageAckGatewayData ackData)
                    MessageAck?.Invoke(ackData);
                else
                    _logger.Warning("MESSAGE_ACK event received but data could not be cast to MessageAckGatewayData");
                return;

            // Channel updates
            case "CHANNEL_PINS_UPDATE":
                if (p.Data is ChannelPinsUpdateGatewayData pinsUpdateData)
                    ChannelPinsUpdate?.Invoke(pinsUpdateData);
                else
                    _logger.Warning("CHANNEL_PINS_UPDATE event received but data could not be cast to ChannelPinsUpdateGatewayData");
                return;

            // Voice events
            case "VOICE_STATE_UPDATE":
                if (p.Data is VoiceStateGatewayData voiceStateData)
                    VoiceStateUpdate?.Invoke(voiceStateData);
                else
                    _logger.Warning("VOICE_STATE_UPDATE event received but data could not be cast to VoiceStateGatewayData");
                return;
            case "VOICE_SERVER_UPDATE":
                if (p.Data is VoiceServerUpdateGatewayData voiceServerData)
                    VoiceServerUpdate?.Invoke(voiceServerData);
                else
                    _logger.Warning("VOICE_SERVER_UPDATE event received but data could not be cast to VoiceServerUpdateGatewayData");
                return;

            // Guildban events
            case "GUILD_BAN_ADD":
                if (p.Data is GuildBanGatewayData banAddData)
                    GuildBanAdd?.Invoke(banAddData);
                else
                    _logger.Warning("GUILD_BAN_ADD event received but data could not be cast to GuildBanGatewayData");
                return;
            case "GUILD_BAN_REMOVE":
                if (p.Data is GuildBanGatewayData banRemoveData)
                    GuildBanRemove?.Invoke(banRemoveData);
                else
                    _logger.Warning("GUILD_BAN_REMOVE event received but data could not be cast to GuildBanGatewayData");
                return;

            // Webhooks
            case "WEBHOOKS_UPDATE":
                if (p.Data is WebhooksUpdateGatewayData webhooksData)
                    WebhooksUpdate?.Invoke(webhooksData);
                else
                    _logger.Warning("WEBHOOKS_UPDATE event received but data could not be cast to WebhooksUpdateGatewayData");
                return;

            // Guild events
            case "GUILD_CREATE":
                if (p.Data is GuildGatewayData guildCreateData)
                    GuildCreate?.Invoke(guildCreateData);
                else
                    _logger.Warning("GUILD_CREATE event received but data could not be cast to GuildGatewayData");
                return;
            case "GUILD_UPDATE":
                if (p.Data is GuildGatewayData guildUpdateData)
                    GuildUpdate?.Invoke(guildUpdateData);
                else
                    _logger.Warning("GUILD_UPDATE event received but data could not be cast to GuildGatewayData");
                return;
            case "GUILD_DELETE":
                if (p.Data is EntityRemovedGatewayData guildDeleteData)
                    GuildDelete?.Invoke(guildDeleteData);
                else
                    _logger.Warning("GUILD_DELETE event received but data could not be cast to EntityRemovedGatewayData");
                return;
            case "GUILD_MEMBER_ADD":
                if (p.Data is GuildMemberGatewayData guildMemberAddData)
                    GuildMemberAdd?.Invoke(guildMemberAddData);
                else
                    _logger.Warning("GUILD_MEMBER_ADD event received but data could not be cast to GuildMemberGatewayData");
                return;
            case "GUILD_MEMBER_UPDATE":
                if (p.Data is GuildMemberGatewayData guildMemberUpdateData)
                    GuildMemberUpdate?.Invoke(guildMemberUpdateData);
                else
                    _logger.Warning("GUILD_MEMBER_UPDATE event received but data could not be cast to GuildMemberGatewayData");
                return;
            case "GUILD_MEMBER_REMOVE":
                if (p.Data is EntityRemovedGatewayData guildMemberRemoveData)
                    GuildMemberRemove?.Invoke(guildMemberRemoveData);
                else
                    _logger.Warning("GUILD_MEMBER_REMOVE event received but data could not be cast to EntityRemovedGatewayData");
                return;
            case "GUILD_ROLE_CREATE":
                if (p.Data is RoleGatewayData guildRoleCreateData)
                    GuildRoleCreate?.Invoke(guildRoleCreateData);
                else
                    _logger.Warning("GUILD_ROLE_CREATE event received but data could not be cast to RoleGatewayData");
                return;
            case "GUILD_ROLE_UPDATE":
                if (p.Data is RoleGatewayData guildRoleUpdateData)
                    GuildRoleUpdate?.Invoke(guildRoleUpdateData);
                else
                    _logger.Warning("GUILD_ROLE_UPDATE event received but data could not be cast to RoleGatewayData");
                return;
            case "GUILD_ROLE_DELETE":
                if (p.Data is EntityRemovedGatewayData guildRoleDeleteData)
                    GuildRoleDelete?.Invoke(guildRoleDeleteData);
                else
                    _logger.Warning("GUILD_ROLE_DELETE event received but data could not be cast to EntityRemovedGatewayData");
                return;

            default:
                _logger.Warning("Unhandled dispatch {Dispatch}", p.Dispatch);
                break;
        }
    }

    private void HandleHello(GatewayPacket packet)
    {
        if (packet.Data is not HelloGatewayData data)
        {
            _logger.Warning("HELLO event received but data could not be cast to HelloGatewayData");
            return;
        }

        // avoid multiple heartbeat threads
        _heartbeatInterval = data.HeartbeatInterval;
        if (!_heartbeatStarted)
        {
            _heartbeatCancellation = new CancellationTokenSource();
            _ = Task.Run(async () => await HandleHeartbeat(_heartbeatCancellation.Token), _heartbeatCancellation.Token);
            _heartbeatStarted = true;
        }
    }

    private void ReEstablishGatewayConnection(ReconnectionInfo? info = null)
    {
        // Synchronous wrapper for backward compatibility
        _ = Task.Run(async () => await ReEstablishGatewayConnectionAsync(info));
    }

    private async Task ReEstablishGatewayConnectionAsync(ReconnectionInfo? info = null)
    {
        if (info is not null)
        {
            if (info.Type is ReconnectionType.Error or ReconnectionType.ByServer)
                _logger.Error("Reconnected with info {Info}", info);
            else
                _logger.Information("Reconnected with info {Info}", info);
        }
        else
        {
            _logger.Warning("Reconnected without connection info");
        }

        // Use semaphore to prevent concurrent reconnection attempts
        if (!await _reconnectLock.WaitAsync(0))
        {
            _logger.Debug("Reconnection already in progress, skipping duplicate attempt");
            return;
        }

        try
        {
            var timeSinceLastAttempt = DateTime.Now - _lastGatewayReEstablishAttempt;
            var requiredDelay = TimeSpan.FromSeconds(_config.ReconnectAttemptDelay);

            if (timeSinceLastAttempt < requiredDelay)
            {
                var remainingDelay = requiredDelay - timeSinceLastAttempt;
                _logger.Warning("Rate limiting reconnection. Waiting {RemainingSeconds:F1} seconds before reconnect attempt.", remainingDelay.TotalSeconds);
                await Task.Delay(remainingDelay);
            }

            _lastGatewayReEstablishAttempt = DateTime.Now;
            _gatewayDuration.Restart();

            _logger.Information("Attempting to reestablish the gateway connection after {Time}ms", _gatewayDuration.ElapsedMilliseconds);

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
        finally
        {
            _reconnectLock.Release();
        }
    }

    private void HandleHeartbeatAck()
    {
        HeartbeatAck?.Invoke();
    }

    private async Task HandleHeartbeat(CancellationToken cancellationToken)
    {
        // Add jitter between 0-500ms to prevent thundering herd
        var jitter = Random.Shared.Next(0, 500);
        _logger.Debug("Starting heartbeat with interval {Interval}ms and jitter {Jitter}ms", _heartbeatInterval, jitter);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_heartbeatInterval + jitter, cancellationToken);

                _logger.Verbose("Sending heartbeat with sequence {Sequence}", _sequence);
                var packet = new HeartbeatPacket()
                {
                    Data = _sequence,
                    OpCode = FluxerOpCode.Heartbeat,
                };
                SendGatewayPacket(packet);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Information("Heartbeat task cancelled");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error in heartbeat handler");
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

    // guild
    public delegate void GuildCreateEvent(GuildGatewayData data);
    public event GuildCreateEvent GuildCreate;

    public delegate void GuildUpdateEvent(GuildGatewayData data);
    public event GuildUpdateEvent GuildUpdate;

    public delegate void GuildDeleteEvent(EntityRemovedGatewayData data);
    public event GuildDeleteEvent GuildDelete;

    // typing
    public delegate void TypingStartEvent(TypingGatewayData data);
    public event TypingStartEvent TypingStart;

    public delegate void TypingStopEvent(TypingGatewayData data);
    public event TypingStopEvent TypingStop;

    // guild member

    public delegate void GuildMemberAddEvent(GuildMemberGatewayData data);
    public event GuildMemberAddEvent GuildMemberAdd;

    public delegate void GuildMemberUpdateEvent(GuildMemberGatewayData data);
    public event GuildMemberUpdateEvent GuildMemberUpdate;

    public delegate void GuildMemberRemoveEvent(EntityRemovedGatewayData data);
    public event GuildMemberRemoveEvent GuildMemberRemove;

    // guild role

    public delegate void GuildRoleCreateEvent(RoleGatewayData data);
    public event GuildRoleCreateEvent GuildRoleCreate;

    public delegate void GuildRoleUpdateEvent(RoleGatewayData data);
    public event GuildRoleUpdateEvent GuildRoleUpdate;

    public delegate void GuildRoleDeleteEvent(EntityRemovedGatewayData data);
    public event GuildRoleDeleteEvent GuildRoleDelete;

    // message reactions

    public delegate void MessageReactionAddEvent(MessageReactionGatewayData data);
    public event MessageReactionAddEvent MessageReactionAdd;

    public delegate void MessageReactionRemoveEvent(MessageReactionGatewayData data);
    public event MessageReactionRemoveEvent MessageReactionRemove;

    public delegate void MessageReactionRemoveAllEvent(EntityRemovedGatewayData data);
    public event MessageReactionRemoveAllEvent MessageReactionRemoveAll;

    public delegate void MessageReactionRemoveEmojiEvent(MessageReactionRemoveEmojiGatewayData data);
    public event MessageReactionRemoveEmojiEvent MessageReactionRemoveEmoji;

    // message bulk operations

    public delegate void MessageDeleteBulkEvent(MessageBulkDeleteGatewayData data);
    public event MessageDeleteBulkEvent MessageDeleteBulk;

    public delegate void MessageAckEvent(MessageAckGatewayData data);
    public event MessageAckEvent MessageAck;

    // channel updates

    public delegate void ChannelPinsUpdateEvent(ChannelPinsUpdateGatewayData data);
    public event ChannelPinsUpdateEvent ChannelPinsUpdate;

    // voice events

    public delegate void VoiceStateUpdateEvent(VoiceStateGatewayData data);
    public event VoiceStateUpdateEvent VoiceStateUpdate;

    public delegate void VoiceServerUpdateEvent(VoiceServerUpdateGatewayData data);
    public event VoiceServerUpdateEvent VoiceServerUpdate;

    // guild ban events

    public delegate void GuildBanAddEvent(GuildBanGatewayData data);
    public event GuildBanAddEvent GuildBanAdd;

    public delegate void GuildBanRemoveEvent(GuildBanGatewayData data);
    public event GuildBanRemoveEvent GuildBanRemove;

    // webhooks

    public delegate void WebhooksUpdateEvent(WebhooksUpdateGatewayData data);
    public event WebhooksUpdateEvent WebhooksUpdate;

    #endregion

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _logger.Information("Disposing GatewayClient");

            // Cancel and dispose heartbeat
            try
            {
                _heartbeatCancellation?.Cancel();
                _heartbeatCancellation?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error cancelling heartbeat during disposal");
            }

            // Dispose WebSocket client
            try
            {
                _gateway?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error disposing WebSocket client");
            }

            // Dispose semaphore
            try
            {
                _reconnectLock?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error disposing reconnect lock");
            }
        }

        _disposed = true;
    }

    ~GatewayClient()
    {
        Dispose(false);
    }

    #endregion
}
