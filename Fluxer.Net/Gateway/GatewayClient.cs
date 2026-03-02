#undef NOPE
using Fluxer.Net.Data.Enums;
using Fluxer.Net.Gateway.Data;
using Fluxer.Net.Gateway.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Websocket.Client;
// ReSharper disable ConstantConditionalAccessQualifier

namespace Fluxer.Net;

/// <summary>
/// WebSocket gateway client for real-time events from the Fluxer platform.
/// Handles bidirectional communication, automatic heartbeats, and reconnection with resume capability.
/// </summary>
/// <remarks>
/// <para>
/// The GatewayClient provides an event-driven architecture for receiving real-time updates
/// from Fluxer, including messages, user presence, guild changes, and more. It implements
/// the Fluxer Gateway protocol with support for:
/// </para>
/// <list type="bullet">
/// <item>Automatic heartbeat mechanism to maintain connection</item>
/// <item>Sequence tracking for ordered event processing</item>
/// <item>Session-based reconnection with resume capability (no event loss)</item>
/// <item>Event filtering via <see cref="FluxerConfig.IgnoredGatewayEvents"/></item>
/// </list>
/// <para>
/// This client should be paired with <see cref="ApiClient"/> for full Fluxer functionality.
/// </para>
/// </remarks>
public partial class GatewayClient : IDisposable
{
    #region Declares
    private FluxerClient _client;
    private WebsocketClient _webSocket;
    private readonly Stopwatch _gatewayDuration = new();

    /// <summary>
    /// Current sequence number for gateway events. Used for resuming connections without data loss.
    /// </summary>
    private int _sequence = 0;

    private bool _heartbeatStarted = false;
    private int _heartbeatInterval = 0;
    private DateTime _lastGatewayReEstablishAttempt = DateTime.Now;
    private int _reconnectAttemptCount = 0;

    /// <summary>
    /// Session ID received from the READY event. Used for resuming connections.
    /// </summary>
    private string _sessionId = "";

    private Logger _logger;
    private CancellationTokenSource? _heartbeatCancellation;
    private readonly SemaphoreSlim _reconnectLock = new(1, 1);
    private bool _disposed = false;
    private bool _isConnecting = false;

    // build error from generated regex
    // temp. removed pending investigation.
    private static readonly Regex PacketSRegex = new(@"(?<=""s""\s*?:\s*?)\d*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    #endregion

    #region Meta
    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayClient"/> class.
    /// </summary>
    /// <param name="token">The authentication token for gateway connection.</param>
    /// <param name="config">Configuration options including gateway URL, event filtering, and reconnection settings.</param>
    /// <remarks>
    /// The client is initialized but not connected. Call <see cref="ConnectAsync"/> to establish the gateway connection.
    /// </remarks>
    internal GatewayClient(FluxerClient client)
    {
        _client = client;
        _logger = client.Config.GatewaySerilog;
        _logger.Information("Initialized Fluxer.Net gateway client ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _client.Config.Version);
    }
    #endregion

    #region Gateway
    /// <summary>
    /// Establishes a WebSocket connection to the Fluxer gateway and sends the IDENTIFY packet.
    /// </summary>
    /// <returns>A task that represents the asynchronous connection operation.</returns>
    /// <remarks>
    /// <para>
    /// This method:
    /// </para>
    /// <list type="number">
    /// <item>Creates a WebSocket connection to the gateway URL specified in configuration</item>
    /// <item>Sets up message and reconnection handlers</item>
    /// <item>Sends an IDENTIFY packet with authentication token and presence</item>
    /// <item>Waits for HELLO event to begin heartbeat mechanism</item>
    /// </list>
    /// <para>
    /// The connection will automatically reconnect on disconnect and attempt to resume the session.
    /// </para>
    /// </remarks>
    public async Task ConnectAsync()
    {
        if (_isConnecting)
        {
            _logger.Warning("Connection attempt already in progress, skipping duplicate ConnectAsync call");
            return;
        }

        _isConnecting = true;
        try
        {
            // Dispose existing gateway connection if present to avoid multiple connections
            if (_webSocket != null)
            {
                try
                {
                    _webSocket.Dispose();
                    _logger.Debug("Disposed existing WebSocket connection before creating new one");
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error disposing existing gateway connection");
                }
            }

            _webSocket = new WebsocketClient(new(_client.Config.FluxerGatewayUrl))
            {
                // IMPORTANT: Do not set ReconnectTimeout here - we manage reconnection manually through the gateway protocol
                IsReconnectionEnabled = false  // Completely disable automatic reconnection
            };
            _webSocket.MessageReceived.Subscribe(x => GatewayMessageHandler(x.Text));
            _webSocket.DisconnectionHappened.Subscribe(info =>
            {
                _isConnecting = false; // Connection attempt finished (failed)
                _logger.Warning("WebSocket disconnected: Type={Type}, CloseStatus={CloseStatus}, CloseStatusDescription={CloseDescription}, Exception={Exception}",
                    info.Type,
                    info.CloseStatus?.ToString() ?? "None",
                    info.CloseStatusDescription ?? "None",
                    info.Exception?.Message ?? "None");

                // Determine if we should attempt reconnection based on close code
                bool shouldReconnect = true;

                // Handle Fluxer-specific close codes
                if (info.CloseStatus.HasValue)
                {
                    shouldReconnect = HandleGatewayCloseCode((int)info.CloseStatus.Value, info.CloseStatusDescription);
                }

                // Manual reconnection handling - only reconnect if we have a valid reason and close code allows it
                if (shouldReconnect && (info.Type == DisconnectionType.ByServer || info.Type == DisconnectionType.Lost || info.Type == DisconnectionType.Error))
                {
                    _logger.Information("Connection lost, manually reconnecting...");
                    _ = Task.Run(async () => await HandleReconnectWithBackoff());
                }
                else if (!shouldReconnect)
                {
                    _logger.Warning("Reconnection disabled due to close code. Manual intervention required.");
                    _reconnectAttemptCount = 0; // Reset backoff counter
                }
            });
            Stopwatch.StartNew();

            _logger.Information("Starting WebSocket connection to {GatewayUrl}", _client.Config.FluxerGatewayUrl);
            await _webSocket.Start();

            // Wait a moment for connection to establish before sending IDENTIFY
            await Task.Delay(100);

            if (_webSocket?.IsRunning != true)
            {
                _logger.Error("WebSocket failed to start properly");
                _isConnecting = false;
                return;
            }

            var login = new GatewayPacket
            {
                OpCode = FluxerOpCode.Identify,
                Data = new IdentifyGatewayData(_client.RawToken)
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "os", "linux" },
                        { "browser", "fluxer-net" },
                        { "device", "fluxer-net" }
                    },
                    IgnoredGatewayEvents = _client.Config.IgnoredGatewayEvents,
                    Presence = _client.Config.Presence
                }
            };

            SendGatewayPacket(login);
            _reconnectAttemptCount = 0; // Reset backoff counter on successful connection
        }
        finally
        {
            // Will be set to false when we receive READY event
        }
    }

    /// <summary>
    /// Sends a gateway packet to the Fluxer WebSocket server.
    /// </summary>
    /// <typeparam name="T">The type of the gateway packet data.</typeparam>
    /// <param name="Data">The packet data to serialize and send.</param>
    /// <remarks>
    /// This method automatically schedules reconnection if sending fails. Packets may be dropped
    /// during reconnection attempts. For critical operations, use the REST API via <see cref="ApiClient"/>.
    /// </remarks>
    public void SendGatewayPacket<T>(T Data)
    {
        // Check if WebSocket is actually connected before trying to send
        if (_webSocket == null || !_webSocket.IsRunning)
        {
            _logger.Warning("Cannot send gateway packet - WebSocket is not connected (IsRunning={IsRunning})", _webSocket?.IsRunning ?? false);
            return;
        }

        var text = JsonConvert.SerializeObject(Data, FluxerClient._serializerSettings);
        _logger.Debug("Sending serialized gateway packet {Enums}", text);
        try
        {
            _webSocket.Send(text);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to send gateway packet. Scheduling reconnection. Some packets may be dropped.");
            // Don't block - schedule reconnection on thread pool
            _ = Task.Run(async () => await HandleReconnectWithBackoff());
        }
    }

    /// <summary>
    /// Handles reconnection with exponential backoff to avoid overwhelming the gateway.
    /// </summary>
    private async Task HandleReconnectWithBackoff()
    {
        _reconnectAttemptCount++;

        // Exponential backoff: 1s, 2s, 4s, 8s, 16s, max 60s
        var baseDelay = Math.Min(Math.Pow(2, _reconnectAttemptCount - 1), 60);
        var jitter = Random.Shared.NextDouble() * 0.3 * baseDelay; // Add up to 30% jitter
        var totalDelay = TimeSpan.FromSeconds(baseDelay + jitter);

        _logger.Information("Reconnection attempt #{Attempt} - waiting {Delay:F1} seconds before reconnecting",
            _reconnectAttemptCount, totalDelay.TotalSeconds);

        await Task.Delay(totalDelay);

        // Check if we have a valid session to resume
        if (!string.IsNullOrEmpty(_sessionId))
        {
            _logger.Information("Attempting to resume existing session");
            await ReEstablishGatewayConnectionAsync();
        }
        else
        {
            _logger.Information("No session to resume, creating fresh connection");
            await ConnectAsync();
        }
    }

    private void GatewayMessageHandler(string message)
    {
        // try deserializing the packet, fallback to regex
        _logger.Verbose("Received raw gateway message {Message}", message);
        try
        {
            // Pre-parse to check for InvalidSession opcode (which has a boolean payload instead of an object)
            var preCheck = JObject.Parse(message);
            var opCode = preCheck["op"]?.Value<int>();
            var dispatch = preCheck["t"]?.Value<string>();

            // Log READY events specifically
            if (dispatch == "READY")
            {
                _logger.Information("Received READY event - connection fully established");
            }

            if (opCode == (int)FluxerOpCode.InvalidSession)
            {
                // InvalidSession has a boolean "d" field indicating if session is resumable
                var canResume = preCheck["d"]?.Value<bool>() ?? false;
                _logger.Warning("Received InvalidSession opcode (resumable: {CanResume}). Need to reconnect with new session.", canResume);

                // For non-resumable session, we need to do a fresh IDENTIFY, not a RESUME
                if (!canResume)
                {
                    // Session is not resumable, need fresh connection
                    _sessionId = ""; // Clear session ID to force IDENTIFY
                    _sequence = 0;   // Reset sequence
                }

                // Close the current connection and reconnect with backoff
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Dispose current connection
                        if (_webSocket != null)
                        {
                            try
                            {
                                _webSocket.Dispose();
                            }
                            catch (Exception ex)
                            {
                                _logger.Warning(ex, "Error disposing gateway after InvalidSession");
                            }
                        }

                        // Reconnect with backoff (will use IDENTIFY or RESUME based on _sessionId)
                        await HandleReconnectWithBackoff();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Failed to reconnect after invalid session");
                    }
                });
                return;
            }

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
                    // Should not reach here due to pre-check above, but handle it anyway
                    _logger.Warning("Received InvalidSession opcode via fallback path, reconnecting");
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
                    _isConnecting = false; // Connection successfully established
                    _reconnectAttemptCount = 0; // Reset backoff counter
                    _logger.Information("Connection established successfully with session ID: {SessionId}", _sessionId);
                    Ready?.Invoke(readyData);
                }
                else
                {
                    _logger.Warning("READY event received but data could not be cast to ReadyGatewayData");
                }
                return;
            case "RESUMED":
                _isConnecting = false; // Connection successfully resumed
                _reconnectAttemptCount = 0; // Reset backoff counter
                _logger.Information("Session resumed successfully");
                Resumed?.Invoke();
                return;
            case "SESSIONS_REPLACE":
                if (p.Data is SessionsReplaceGatewayData sessionsReplaceData)
                    SessionsReplace?.Invoke(sessionsReplaceData);
                else
                    _logger.Warning("SESSIONS_REPLACE event received but data could not be cast to SessionsReplaceGatewayData");
                return;

            // User settings events
            case "USER_SETTINGS_UPDATE":
                if (p.Data is UserSettingsUpdateGatewayData userSettingsData)
                    UserSettingsUpdate?.Invoke(userSettingsData);
                else
                    _logger.Warning("USER_SETTINGS_UPDATE event received but data could not be cast to UserSettingsUpdateGatewayData");
                return;
            case "USER_GUILD_SETTINGS_UPDATE":
                if (p.Data is UserGuildSettingsUpdateGatewayData userGuildSettingsData)
                    UserGuildSettingsUpdate?.Invoke(userGuildSettingsData);
                else
                    _logger.Warning("USER_GUILD_SETTINGS_UPDATE event received but data could not be cast to UserGuildSettingsUpdateGatewayData");
                return;
            case "USER_PINNED_DMS_UPDATE":
                if (p.Data is UserPinnedDmsUpdateGatewayData pinnedDmsData)
                    UserPinnedDmsUpdate?.Invoke(pinnedDmsData);
                else
                    _logger.Warning("USER_PINNED_DMS_UPDATE event received but data could not be cast to UserPinnedDmsUpdateGatewayData");
                return;
            case "USER_NOTE_UPDATE":
                if (p.Data is UserNoteUpdateGatewayData userNoteData)
                    UserNoteUpdate?.Invoke(userNoteData);
                else
                    _logger.Warning("USER_NOTE_UPDATE event received but data could not be cast to UserNoteUpdateGatewayData");
                return;
            case "AUTH_SESSION_CHANGE":
                if (p.Data is AuthSessionChangeGatewayData authSessionData)
                    AuthSessionChange?.Invoke(authSessionData);
                else
                    _logger.Warning("AUTH_SESSION_CHANGE event received but data could not be cast to AuthSessionChangeGatewayData");
                return;

            // Message events
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
                if (p.Data is ChannelGatewayData channelDeleteData)
                    ChannelDelete?.Invoke(channelDeleteData);
                else
                    _logger.Warning("CHANNEL_DELETE event received but data could not be cast to ChannelGatewayData");
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

            // Saved messages
            case "SAVED_MESSAGE_CREATE":
                if (p.Data is SavedMessageGatewayData savedMessageCreateData)
                    SavedMessageCreate?.Invoke(savedMessageCreateData);
                else
                    _logger.Warning("SAVED_MESSAGE_CREATE event received but data could not be cast to SavedMessageGatewayData");
                return;
            case "SAVED_MESSAGE_DELETE":
                if (p.Data is SavedMessageGatewayData savedMessageDeleteData)
                    SavedMessageDelete?.Invoke(savedMessageDeleteData);
                else
                    _logger.Warning("SAVED_MESSAGE_DELETE event received but data could not be cast to SavedMessageGatewayData");
                return;
            case "RECENT_MENTION_DELETE":
                if (p.Data is RecentMentionDeleteGatewayData recentMentionData)
                    RecentMentionDelete?.Invoke(recentMentionData);
                else
                    _logger.Warning("RECENT_MENTION_DELETE event received but data could not be cast to RecentMentionDeleteGatewayData");
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
            case "CHANNEL_UPDATE_BULK":
                if (p.Data is ChannelUpdateBulkGatewayData channelBulkUpdateData)
                    ChannelUpdateBulk?.Invoke(channelBulkUpdateData);
                else
                    _logger.Warning("CHANNEL_UPDATE_BULK event received but data could not be cast to ChannelUpdateBulkGatewayData");
                return;
            case "CHANNEL_RECIPIENT_ADD":
                if (p.Data is ChannelRecipientGatewayData recipientAddData)
                    ChannelRecipientAdd?.Invoke(recipientAddData);
                else
                    _logger.Warning("CHANNEL_RECIPIENT_ADD event received but data could not be cast to ChannelRecipientGatewayData");
                return;
            case "CHANNEL_RECIPIENT_REMOVE":
                if (p.Data is ChannelRecipientGatewayData recipientRemoveData)
                    ChannelRecipientRemove?.Invoke(recipientRemoveData);
                else
                    _logger.Warning("CHANNEL_RECIPIENT_REMOVE event received but data could not be cast to ChannelRecipientGatewayData");
                return;
            case "CHANNEL_PINS_UPDATE":
                if (p.Data is ChannelPinsUpdateGatewayData pinsUpdateData)
                    ChannelPinsUpdate?.Invoke(pinsUpdateData);
                else
                    _logger.Warning("CHANNEL_PINS_UPDATE event received but data could not be cast to ChannelPinsUpdateGatewayData");
                return;
            case "CHANNEL_PINS_ACK":
                if (p.Data is ChannelPinsAckGatewayData pinsAckData)
                    ChannelPinsAck?.Invoke(pinsAckData);
                else
                    _logger.Warning("CHANNEL_PINS_ACK event received but data could not be cast to ChannelPinsAckGatewayData");
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
                else if (p.Data is GuildDeleteGatewayData unavailableGuildData)
                {
                    // GUILD_CREATE with unavailable:true means the guild exists but is temporarily unavailable (outage).
                    // Synthesize a minimal GuildGatewayData so consumers receive the event consistently.
                    _logger.Debug("GUILD_CREATE received for unavailable guild {GuildId}, synthesizing GuildGatewayData", unavailableGuildData.Id);
                    GuildCreate?.Invoke(new GuildGatewayData { Id = unavailableGuildData.Id, Unavailable = true });
                }
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
                if (p.Data is GuildDeleteGatewayData guildDeleteData)
                    GuildDelete?.Invoke(guildDeleteData);
                else
                    _logger.Warning("GUILD_DELETE event received but data could not be cast to GuildDeleteGatewayData");
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
                if (p.Data is GuildRoleGatewayData guildRoleCreateData)
                    GuildRoleCreate?.Invoke(guildRoleCreateData);
                else
                    _logger.Warning("GUILD_ROLE_CREATE event received but data could not be cast to GuildRoleGatewayData");
                return;
            case "GUILD_ROLE_UPDATE":
                if (p.Data is GuildRoleGatewayData guildRoleUpdateData)
                    GuildRoleUpdate?.Invoke(guildRoleUpdateData);
                else
                    _logger.Warning("GUILD_ROLE_UPDATE event received but data could not be cast to GuildRoleGatewayData");
                return;
            case "GUILD_ROLE_DELETE":
                if (p.Data is GuildRoleDeleteGatewayData guildRoleDeleteData)
                    GuildRoleDelete?.Invoke(guildRoleDeleteData);
                else
                    _logger.Warning("GUILD_ROLE_DELETE event received but data could not be cast to GuildRoleDeleteGatewayData");
                return;
            case "GUILD_ROLE_UPDATE_BULK":
                if (p.Data is GuildRoleUpdateBulkGatewayData guildRoleBulkData)
                    GuildRoleUpdateBulk?.Invoke(guildRoleBulkData);
                else
                    _logger.Warning("GUILD_ROLE_UPDATE_BULK event received but data could not be cast to GuildRoleUpdateBulkGatewayData");
                return;
            case "GUILD_EMOJIS_UPDATE":
                if (p.Data is GuildEmojisUpdateGatewayData guildEmojisUpdateData)
                    GuildEmojisUpdate?.Invoke(guildEmojisUpdateData);
                else
                    _logger.Warning("GUILD_EMOJIS_UPDATE event received but data could not be cast to GuildEmojisUpdateGatewayData");
                return;
            case "GUILD_STICKERS_UPDATE":
                if (p.Data is GuildStickersUpdateGatewayData guildStickersUpdateData)
                    GuildStickersUpdate?.Invoke(guildStickersUpdateData);
                else
                    _logger.Warning("GUILD_STICKERS_UPDATE event received but data could not be cast to GuildStickersUpdateGatewayData");
                return;

            // Relationship events
            case "RELATIONSHIP_ADD":
                if (p.Data is RelationshipGatewayData relationshipAddData)
                    RelationshipAdd?.Invoke(relationshipAddData);
                else
                    _logger.Warning("RELATIONSHIP_ADD event received but data could not be cast to RelationshipGatewayData");
                return;
            case "RELATIONSHIP_UPDATE":
                if (p.Data is RelationshipGatewayData relationshipUpdateData)
                    RelationshipUpdate?.Invoke(relationshipUpdateData);
                else
                    _logger.Warning("RELATIONSHIP_UPDATE event received but data could not be cast to RelationshipGatewayData");
                return;
            case "RELATIONSHIP_REMOVE":
                if (p.Data is RelationshipGatewayData relationshipRemoveData)
                    RelationshipRemove?.Invoke(relationshipRemoveData);
                else
                    _logger.Warning("RELATIONSHIP_REMOVE event received but data could not be cast to RelationshipGatewayData");
                return;

            // Favorite meme events
            case "FAVORITE_MEME_CREATE":
                if (p.Data is FavoriteMemeGatewayData favoriteMemeCreateData)
                    FavoriteMemeCreate?.Invoke(favoriteMemeCreateData);
                else
                    _logger.Warning("FAVORITE_MEME_CREATE event received but data could not be cast to FavoriteMemeGatewayData");
                return;
            case "FAVORITE_MEME_UPDATE":
                if (p.Data is FavoriteMemeGatewayData favoriteMemeUpdateData)
                    FavoriteMemeUpdate?.Invoke(favoriteMemeUpdateData);
                else
                    _logger.Warning("FAVORITE_MEME_UPDATE event received but data could not be cast to FavoriteMemeGatewayData");
                return;
            case "FAVORITE_MEME_DELETE":
                if (p.Data is FavoriteMemeGatewayData favoriteMemeDeleteData)
                    FavoriteMemeDelete?.Invoke(favoriteMemeDeleteData);
                else
                    _logger.Warning("FAVORITE_MEME_DELETE event received but data could not be cast to FavoriteMemeGatewayData");
                return;

            // Call events
            case "CALL_CREATE":
                if (p.Data is CallGatewayData callCreateData)
                    CallCreate?.Invoke(callCreateData);
                else
                    _logger.Warning("CALL_CREATE event received but data could not be cast to CallGatewayData");
                return;
            case "CALL_UPDATE":
                if (p.Data is CallGatewayData callUpdateData)
                    CallUpdate?.Invoke(callUpdateData);
                else
                    _logger.Warning("CALL_UPDATE event received but data could not be cast to CallGatewayData");
                return;
            case "CALL_DELETE":
                if (p.Data is CallGatewayData callDeleteData)
                    CallDelete?.Invoke(callDeleteData);
                else
                    _logger.Warning("CALL_DELETE event received but data could not be cast to CallGatewayData");
                return;

            // Invite events
            case "INVITE_CREATE":
                if (p.Data is InviteGatewayData inviteCreateData)
                    InviteCreate?.Invoke(inviteCreateData);
                else
                    _logger.Warning("INVITE_CREATE event received but data could not be cast to InviteGatewayData");
                return;
            case "INVITE_DELETE":
                if (p.Data is InviteGatewayData inviteDeleteData)
                    InviteDelete?.Invoke(inviteDeleteData);
                else
                    _logger.Warning("INVITE_DELETE event received but data could not be cast to InviteGatewayData");
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

    private async Task ReEstablishGatewayConnectionAsync(ReconnectionInfo? info = null)
    {
        _logger.Information("Attempting to reestablish gateway connection...");


        // Use semaphore to prevent concurrent reconnection attempts
        if (!await _reconnectLock.WaitAsync(0))
        {
            _logger.Debug("Reconnection already in progress, skipping duplicate attempt");
            return;
        }

        try
        {
            // Check if we have a valid session to resume
            if (string.IsNullOrEmpty(_sessionId))
            {
                _logger.Information("No valid session ID available. Sending IDENTIFY instead of RESUME.");

                // Send fresh IDENTIFY packet since we don't have a session to resume
                var identifyPacket = new GatewayPacket
                {
                    OpCode = FluxerOpCode.Identify,
                    Data = new IdentifyGatewayData(_client.RawToken)
                    {
                        Properties = new Dictionary<string, string>
                        {
                            { "os", Environment.OSVersion.Platform.ToString() },
                            { "browser", "Fluxer.Net" },
                            { "device", "Fluxer.Net" }
                        },
                        IgnoredGatewayEvents = _client.Config.IgnoredGatewayEvents,
                        Presence = _client.Config.Presence
                    }
                };
                SendGatewayPacket(identifyPacket);
                return;
            }

            var timeSinceLastAttempt = DateTime.Now - _lastGatewayReEstablishAttempt;
            var requiredDelay = TimeSpan.FromSeconds(_client.Config.ReconnectAttemptDelay);

            if (timeSinceLastAttempt < requiredDelay)
            {
                var remainingDelay = requiredDelay - timeSinceLastAttempt;
                _logger.Warning("Rate limiting reconnection. Waiting {RemainingSeconds:F1} seconds before reconnect attempt.", remainingDelay.TotalSeconds);
                await Task.Delay(remainingDelay);
            }

            _lastGatewayReEstablishAttempt = DateTime.Now;
            _gatewayDuration.Restart();

            _logger.Information("Attempting to resume session {SessionId} with sequence {Sequence}", _sessionId, _sequence);

            var packet = new GatewayPacket()
            {
                OpCode = FluxerOpCode.Resume,
                Data = new ReconnectGatewayData()
                {
                    Sequence = _sequence,
                    SessionId = _sessionId,
                    Token = _client.RawToken
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

    /// <summary>
    /// Handles Fluxer-specific gateway close codes and determines appropriate action.
    /// </summary>
    /// <param name="closeCode">The WebSocket close code received from the gateway.</param>
    /// <param name="description">Optional description provided with the close event.</param>
    /// <returns>True if reconnection should be attempted; false if reconnection should be prevented.</returns>
    private bool HandleGatewayCloseCode(int closeCode, string? description)
    {
        // Check if this is a Fluxer-specific close code (4000-4014)
        if (!Enum.IsDefined(typeof(FluxerCloseCode), closeCode))
        {
            _logger.Debug("Received non-Fluxer close code {CloseCode}, using default reconnection logic", closeCode);
            return true; // Allow reconnection for non-Fluxer codes
        }

        var fluxerCode = (FluxerCloseCode)closeCode;
        _logger.Warning("Received Fluxer close code: {CloseCode} ({CodeValue}) - {Description}",
            fluxerCode, closeCode, description ?? "No description");

        switch (fluxerCode)
        {
            case FluxerCloseCode.UnknownError:
                // Unknown error - can retry connection
                _logger.Information("Unknown error occurred, connection will be retried");
                return true;

            case FluxerCloseCode.UnknownOpcode:
                // Invalid opcode sent - this is a client bug, but we can retry
                _logger.Error("Invalid gateway opcode was sent. This indicates a client implementation error.");
                return true;

            case FluxerCloseCode.DecodeError:
                // Invalid payload sent - this is a client bug, but we can retry
                _logger.Error("Invalid payload sent that could not be decoded. This indicates a client serialization error.");
                return true;

            case FluxerCloseCode.NotAuthenticated:
                // Sent payload before authenticating - this is a client bug
                _logger.Error("Payload was sent before authentication. This indicates a client sequencing error.");
                return true;

            case FluxerCloseCode.AuthenticationFailed:
                // Invalid token - DO NOT retry, clear session and notify
                _logger.Error("Authentication failed - token is invalid. Cannot reconnect with current credentials.");
                _sessionId = "";
                _sequence = 0;
                // Stop heartbeat to prevent further connection attempts
                try
                {
                    _heartbeatCancellation?.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error cancelling heartbeat after authentication failure");
                }
                return false; // DO NOT reconnect

            case FluxerCloseCode.AlreadyAuthenticated:
                // Sent auth payload after already authenticating - this is a client bug
                _logger.Error("Authentication payload sent after already authenticated. This indicates a client sequencing error.");
                return true;

            case FluxerCloseCode.InvalidSequence:
                // Invalid sequence in RESUME - session cannot be resumed, must create new session
                _logger.Warning("Invalid sequence number in RESUME packet. Session cannot be resumed, will create new session.");
                _sessionId = ""; // Clear session ID to force IDENTIFY on reconnect
                _sequence = 0;
                return true; // Can reconnect with fresh IDENTIFY

            case FluxerCloseCode.RateLimited:
                // Rate limited - need to slow down
                _logger.Warning("Gateway rate limited. Slowing down reconnection attempts.");
                // The reconnection delay will be handled by ReEstablishGatewayConnectionAsync
                return true;

            case FluxerCloseCode.SessionTimeout:
                // Session expired - must create new session
                _logger.Information("Session timed out. Will create new session on reconnection.");
                _sessionId = ""; // Clear session ID to force IDENTIFY on reconnect
                _sequence = 0;
                return true;

            case FluxerCloseCode.InvalidShard:
                // Invalid shard configuration - DO NOT retry without fixing configuration
                _logger.Error("Invalid shard specified. Check gateway configuration before reconnecting.");
                // Stop heartbeat to prevent automatic reconnection
                try
                {
                    _heartbeatCancellation?.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error cancelling heartbeat after invalid shard error");
                }
                return false; // DO NOT reconnect

            case FluxerCloseCode.ShardingRequired:
                // Sharding is required for this bot - DO NOT retry without sharding
                _logger.Error("Sharding is required for this bot. Gateway connection requires shard configuration.");
                // Stop heartbeat to prevent automatic reconnection
                try
                {
                    _heartbeatCancellation?.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error cancelling heartbeat after sharding required error");
                }
                return false; // DO NOT reconnect

            case FluxerCloseCode.InvalidApiVersion:
                // Invalid API version - DO NOT retry, this is a configuration error
                _logger.Error("Invalid API version specified. Update the library or check gateway configuration.");
                // Stop heartbeat to prevent automatic reconnection
                try
                {
                    _heartbeatCancellation?.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error cancelling heartbeat after invalid API version error");
                }
                return false; // DO NOT reconnect

            case FluxerCloseCode.InvalidIntents:
                // Invalid intents specified - DO NOT retry without fixing configuration
                _logger.Error("Invalid gateway intents specified. Check gateway configuration.");
                // Stop heartbeat to prevent automatic reconnection
                try
                {
                    _heartbeatCancellation?.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error cancelling heartbeat after invalid intents error");
                }
                return false; // DO NOT reconnect

            case FluxerCloseCode.DisallowedIntents:
                // Disallowed intents - DO NOT retry, requires verification/approval
                _logger.Error("Disallowed gateway intents specified. These intents require verification or approval from Fluxer.");
                // Stop heartbeat to prevent automatic reconnection
                try
                {
                    _heartbeatCancellation?.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error cancelling heartbeat after disallowed intents error");
                }
                return false; // DO NOT reconnect

            default:
                _logger.Warning("Unhandled Fluxer close code: {CloseCode}", fluxerCode);
                return true; // Default to allowing reconnection
        }
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

    /// <summary>
    /// Updates the current user's presence status on the gateway.
    /// </summary>
    /// <param name="status">The new status to set (Online, Idle, DoNotDisturb, Invisible).</param>
    /// <remarks>
    /// This sends a PRESENCE_UPDATE packet to the gateway. Other users will see the status change
    /// in real-time through PRESENCE_UPDATE events.
    /// </remarks>
    public void SetStatus(Status status)
    {
        var packet = new GatewayPacket()
        {
            Data = new PresenceUpdateGatewayData(status),
            OpCode = FluxerOpCode.PresenceUpdate
        };
        SendGatewayPacket(packet);
    }


    #region Events

    // ============================================================================
    // Gateway Lifecycle Events
    // ============================================================================

    /// <summary>
    /// Delegate for heartbeat acknowledgment events from the gateway.
    /// </summary>
    public delegate void HeartbeatAckEvent();

    /// <summary>
    /// Occurs when the gateway acknowledges a heartbeat packet.
    /// </summary>
    public event HeartbeatAckEvent HeartbeatAck;

    /// <summary>
    /// Delegate for the READY event fired when the gateway connection is established.
    /// </summary>
    /// <param name="data">The ready event data including session ID, user info, and initial state.</param>
    public delegate void ReadyEvent(ReadyGatewayData data);

    /// <summary>
    /// Occurs when the gateway connection is established and initial data is received.
    /// Contains session ID, current user information, and initial guilds/channels/DMs.
    /// </summary>
    public event ReadyEvent Ready;

    /// <summary>
    /// Delegate for the RESUMED event fired when a session is successfully resumed.
    /// </summary>
    public delegate void ResumedEvent();

    /// <summary>
    /// Occurs when a disconnected session is successfully resumed without data loss.
    /// </summary>
    public event ResumedEvent Resumed;

    /// <summary>
    /// Delegate for SESSIONS_REPLACE event when auth sessions are replaced.
    /// </summary>
    /// <param name="data">The sessions replace data.</param>
    public delegate void SessionsReplaceEvent(SessionsReplaceGatewayData data);

    /// <summary>
    /// Occurs when auth sessions are replaced.
    /// </summary>
    public event SessionsReplaceEvent SessionsReplace;

    // ============================================================================
    // User Settings Events
    // ============================================================================

    /// <summary>
    /// Delegate for USER_SETTINGS_UPDATE events when user settings are updated.
    /// </summary>
    /// <param name="data">The user settings data.</param>
    public delegate void UserSettingsUpdateEvent(UserSettingsUpdateGatewayData data);

    /// <summary>
    /// Occurs when user settings are updated.
    /// </summary>
    public event UserSettingsUpdateEvent UserSettingsUpdate;

    /// <summary>
    /// Delegate for USER_GUILD_SETTINGS_UPDATE events when user guild settings are updated.
    /// </summary>
    /// <param name="data">The user guild settings data.</param>
    public delegate void UserGuildSettingsUpdateEvent(UserGuildSettingsUpdateGatewayData data);

    /// <summary>
    /// Occurs when user guild settings are updated.
    /// </summary>
    public event UserGuildSettingsUpdateEvent UserGuildSettingsUpdate;

    /// <summary>
    /// Delegate for USER_PINNED_DMS_UPDATE events when pinned DMs are updated.
    /// </summary>
    /// <param name="data">The pinned DMs data.</param>
    public delegate void UserPinnedDmsUpdateEvent(UserPinnedDmsUpdateGatewayData data);

    /// <summary>
    /// Occurs when pinned DMs are updated.
    /// </summary>
    public event UserPinnedDmsUpdateEvent UserPinnedDmsUpdate;

    /// <summary>
    /// Delegate for USER_NOTE_UPDATE events when a user note is updated.
    /// </summary>
    /// <param name="data">The user note data.</param>
    public delegate void UserNoteUpdateEvent(UserNoteUpdateGatewayData data);

    /// <summary>
    /// Occurs when a user note is updated.
    /// </summary>
    public event UserNoteUpdateEvent UserNoteUpdate;

    /// <summary>
    /// Delegate for AUTH_SESSION_CHANGE events when an auth session changes.
    /// </summary>
    /// <param name="data">The auth session data.</param>
    public delegate void AuthSessionChangeEvent(AuthSessionChangeGatewayData data);

    /// <summary>
    /// Occurs when an auth session changes.
    /// </summary>
    public event AuthSessionChangeEvent AuthSessionChange;

    // ============================================================================
    // Message Events
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_CREATE events when a new message is sent.
    /// </summary>
    /// <param name="data">The message data including content, author, channel, etc.</param>
    public delegate void MessageCreateEvent(MessageGatewayData data);

    /// <summary>
    /// Occurs when a new message is created in any channel the user has access to.
    /// </summary>
    public event MessageCreateEvent MessageCreate;

    /// <summary>
    /// Delegate for MESSAGE_UPDATE events when a message is edited.
    /// </summary>
    /// <param name="data">The updated message data.</param>
    public delegate void MessageUpdateEvent(MessageGatewayData data);

    /// <summary>
    /// Occurs when a message is edited.
    /// </summary>
    public event MessageUpdateEvent MessageUpdate;

    /// <summary>
    /// Delegate for MESSAGE_DELETE events when a message is deleted.
    /// </summary>
    /// <param name="data">The deleted entity data containing message and channel IDs.</param>
    public delegate void MessageDeleteEvent(EntityRemovedGatewayData data);

    /// <summary>
    /// Occurs when a message is deleted.
    /// </summary>
    public event MessageDeleteEvent MessageDelete;

    // ============================================================================
    // Channel Events
    // ============================================================================

    /// <summary>
    /// Delegate for CHANNEL_CREATE events when a channel is created.
    /// </summary>
    /// <param name="data">The channel data.</param>
    public delegate void ChannelCreateEvent(ChannelGatewayData data);

    /// <summary>
    /// Occurs when a channel is created in a guild.
    /// </summary>
    public event ChannelCreateEvent ChannelCreate;

    /// <summary>
    /// Delegate for CHANNEL_UPDATE events when a channel is updated.
    /// </summary>
    /// <param name="data">The updated channel data.</param>
    public delegate void ChannelUpdateEvent(ChannelGatewayData data);

    /// <summary>
    /// Occurs when a channel is updated.
    /// </summary>
    public event ChannelUpdateEvent ChannelUpdate;

    /// <summary>
    /// Delegate for CHANNEL_DELETE events when a channel is deleted.
    /// </summary>
    /// <param name="data">The deleted channel data.</param>
    public delegate void ChannelDeleteEvent(ChannelGatewayData data);

    /// <summary>
    /// Occurs when a channel is deleted.
    /// </summary>
    public event ChannelDeleteEvent ChannelDelete;

    // ============================================================================
    // User Events
    // ============================================================================

    /// <summary>
    /// Delegate for USER_UPDATE events when the current user's data is updated.
    /// </summary>
    /// <param name="data">The updated user data.</param>
    public delegate void UserUpdateEvent(UserGatewayData data);

    /// <summary>
    /// Occurs when the current user's account is updated.
    /// </summary>
    public event UserUpdateEvent UserUpdate;

    // ============================================================================
    // Presence Events
    // ============================================================================

    /// <summary>
    /// Delegate for PRESENCE_UPDATE events when a user's presence changes.
    /// </summary>
    /// <param name="data">The presence data.</param>
    public delegate void PresenceUpdateEvent(PresenceGatewayData data);

    /// <summary>
    /// Occurs when a user's presence (online status) changes.
    /// </summary>
    public event PresenceUpdateEvent PresenceUpdate;

    // ============================================================================
    // Typing Events
    // ============================================================================

    /// <summary>
    /// Delegate for TYPING_START events when a user starts typing.
    /// </summary>
    /// <param name="data">The typing indicator data.</param>
    public delegate void TypingStartEvent(TypingGatewayData data);

    /// <summary>
    /// Occurs when a user starts typing in a channel.
    /// </summary>
    public event TypingStartEvent TypingStart;

    /// <summary>
    /// Delegate for TYPING_STOP events when a user stops typing.
    /// </summary>
    /// <param name="data">The typing indicator data.</param>
    public delegate void TypingStopEvent(TypingGatewayData data);

    /// <summary>
    /// Occurs when a user stops typing in a channel.
    /// </summary>
    public event TypingStopEvent TypingStop;

    // ============================================================================
    // Message Reaction Events
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_REACTION_ADD events when a reaction is added.
    /// </summary>
    /// <param name="data">The reaction data.</param>
    public delegate void MessageReactionAddEvent(MessageReactionGatewayData data);

    /// <summary>
    /// Occurs when a user adds a reaction to a message.
    /// </summary>
    public event MessageReactionAddEvent MessageReactionAdd;

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE events when a reaction is removed.
    /// </summary>
    /// <param name="data">The reaction data.</param>
    public delegate void MessageReactionRemoveEvent(MessageReactionGatewayData data);

    /// <summary>
    /// Occurs when a user removes a reaction from a message.
    /// </summary>
    public event MessageReactionRemoveEvent MessageReactionRemove;

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_ALL events when all reactions are cleared.
    /// </summary>
    /// <param name="data">The entity data containing message and channel IDs.</param>
    public delegate void MessageReactionRemoveAllEvent(EntityRemovedGatewayData data);

    /// <summary>
    /// Occurs when all reactions are removed from a message.
    /// </summary>
    public event MessageReactionRemoveAllEvent MessageReactionRemoveAll;

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_EMOJI events when all reactions of a specific emoji are removed.
    /// </summary>
    /// <param name="data">The reaction removal data.</param>
    public delegate void MessageReactionRemoveEmojiEvent(MessageReactionRemoveEmojiGatewayData data);

    /// <summary>
    /// Occurs when all instances of a specific emoji are removed from a message.
    /// </summary>
    public event MessageReactionRemoveEmojiEvent MessageReactionRemoveEmoji;

    // ============================================================================
    // Saved Messages and Mentions
    // ============================================================================

    /// <summary>
    /// Delegate for SAVED_MESSAGE_CREATE events when a message is saved.
    /// </summary>
    /// <param name="data">The saved message data.</param>
    public delegate void SavedMessageCreateEvent(SavedMessageGatewayData data);

    /// <summary>
    /// Occurs when a message is saved.
    /// </summary>
    public event SavedMessageCreateEvent SavedMessageCreate;

    /// <summary>
    /// Delegate for SAVED_MESSAGE_DELETE events when a saved message is deleted.
    /// </summary>
    /// <param name="data">The saved message data.</param>
    public delegate void SavedMessageDeleteEvent(SavedMessageGatewayData data);

    /// <summary>
    /// Occurs when a saved message is deleted.
    /// </summary>
    public event SavedMessageDeleteEvent SavedMessageDelete;

    /// <summary>
    /// Delegate for RECENT_MENTION_DELETE events when a recent mention is deleted.
    /// </summary>
    /// <param name="data">The recent mention data.</param>
    public delegate void RecentMentionDeleteEvent(RecentMentionDeleteGatewayData data);

    /// <summary>
    /// Occurs when a recent mention is deleted.
    /// </summary>
    public event RecentMentionDeleteEvent RecentMentionDelete;

    // ============================================================================
    // Message Bulk Operations
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_DELETE_BULK events when multiple messages are deleted at once.
    /// </summary>
    /// <param name="data">The bulk delete data containing message IDs.</param>
    public delegate void MessageDeleteBulkEvent(MessageBulkDeleteGatewayData data);

    /// <summary>
    /// Occurs when multiple messages are deleted in bulk (e.g., purge operation).
    /// </summary>
    public event MessageDeleteBulkEvent MessageDeleteBulk;

    /// <summary>
    /// Delegate for MESSAGE_ACK events when a message is acknowledged as read.
    /// </summary>
    /// <param name="data">The acknowledgment data.</param>
    public delegate void MessageAckEvent(MessageAckGatewayData data);

    /// <summary>
    /// Occurs when a message is acknowledged as read by the current user.
    /// </summary>
    public event MessageAckEvent MessageAck;

    // ============================================================================
    // Channel Update Events
    // ============================================================================

    /// <summary>
    /// Delegate for CHANNEL_PINS_UPDATE events when channel pins are updated.
    /// </summary>
    /// <param name="data">The pins update data.</param>
    public delegate void ChannelPinsUpdateEvent(ChannelPinsUpdateGatewayData data);

    /// <summary>
    /// Occurs when a message is pinned or unpinned in a channel.
    /// </summary>
    public event ChannelPinsUpdateEvent ChannelPinsUpdate;

    /// <summary>
    /// Delegate for CHANNEL_PINS_ACK events when channel pins are acknowledged.
    /// </summary>
    /// <param name="data">The pins acknowledgment data.</param>
    public delegate void ChannelPinsAckEvent(ChannelPinsAckGatewayData data);

    /// <summary>
    /// Occurs when channel pins are acknowledged.
    /// </summary>
    public event ChannelPinsAckEvent ChannelPinsAck;

    /// <summary>
    /// Delegate for CHANNEL_UPDATE_BULK events when multiple channels are updated.
    /// </summary>
    /// <param name="data">The bulk channel update data.</param>
    public delegate void ChannelUpdateBulkEvent(ChannelUpdateBulkGatewayData data);

    /// <summary>
    /// Occurs when multiple channels are updated at once.
    /// </summary>
    public event ChannelUpdateBulkEvent ChannelUpdateBulk;

    /// <summary>
    /// Delegate for CHANNEL_RECIPIENT_ADD events when a recipient is added to a channel.
    /// </summary>
    /// <param name="data">The channel recipient data.</param>
    public delegate void ChannelRecipientAddEvent(ChannelRecipientGatewayData data);

    /// <summary>
    /// Occurs when a recipient is added to a group DM or channel.
    /// </summary>
    public event ChannelRecipientAddEvent ChannelRecipientAdd;

    /// <summary>
    /// Delegate for CHANNEL_RECIPIENT_REMOVE events when a recipient is removed from a channel.
    /// </summary>
    /// <param name="data">The channel recipient data.</param>
    public delegate void ChannelRecipientRemoveEvent(ChannelRecipientGatewayData data);

    /// <summary>
    /// Occurs when a recipient is removed from a group DM or channel.
    /// </summary>
    public event ChannelRecipientRemoveEvent ChannelRecipientRemove;

    // ============================================================================
    // Voice Events
    // ============================================================================

    /// <summary>
    /// Delegate for VOICE_STATE_UPDATE events when a user's voice state changes.
    /// </summary>
    /// <param name="data">The voice state data.</param>
    public delegate void VoiceStateUpdateEvent(VoiceStateGatewayData data);

    /// <summary>
    /// Occurs when a user joins, leaves, or updates their state in a voice channel.
    /// </summary>
    public event VoiceStateUpdateEvent VoiceStateUpdate;

    /// <summary>
    /// Delegate for VOICE_SERVER_UPDATE events for voice connection data.
    /// </summary>
    /// <param name="data">The voice server data.</param>
    public delegate void VoiceServerUpdateEvent(VoiceServerUpdateGatewayData data);

    /// <summary>
    /// Occurs when voice server information is updated (used for establishing voice connections).
    /// </summary>
    public event VoiceServerUpdateEvent VoiceServerUpdate;

    // ============================================================================
    // Guild Ban Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_BAN_ADD events when a user is banned from a guild.
    /// </summary>
    /// <param name="data">The ban data.</param>
    public delegate void GuildBanAddEvent(GuildBanGatewayData data);

    /// <summary>
    /// Occurs when a user is banned from a guild.
    /// </summary>
    public event GuildBanAddEvent GuildBanAdd;

    /// <summary>
    /// Delegate for GUILD_BAN_REMOVE events when a user is unbanned from a guild.
    /// </summary>
    /// <param name="data">The ban data.</param>
    public delegate void GuildBanRemoveEvent(GuildBanGatewayData data);

    /// <summary>
    /// Occurs when a user is unbanned from a guild.
    /// </summary>
    public event GuildBanRemoveEvent GuildBanRemove;

    // ============================================================================
    // Webhook Events
    // ============================================================================

    /// <summary>
    /// Delegate for WEBHOOKS_UPDATE events when webhooks in a channel are updated.
    /// </summary>
    /// <param name="data">The webhooks update data.</param>
    public delegate void WebhooksUpdateEvent(WebhooksUpdateGatewayData data);

    /// <summary>
    /// Occurs when webhooks are created, updated, or deleted in a channel.
    /// </summary>
    public event WebhooksUpdateEvent WebhooksUpdate;

    // ============================================================================
    // Guild Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_CREATE events when a guild becomes available.
    /// </summary>
    /// <param name="data">The guild data.</param>
    public delegate void GuildCreateEvent(GuildGatewayData data);

    /// <summary>
    /// Occurs when the bot joins a guild or when a guild becomes available after an outage.
    /// </summary>
    public event GuildCreateEvent GuildCreate;

    /// <summary>
    /// Delegate for GUILD_UPDATE events when guild properties are updated.
    /// </summary>
    /// <param name="data">The updated guild data.</param>
    public delegate void GuildUpdateEvent(GuildGatewayData data);

    /// <summary>
    /// Occurs when a guild is updated (name, icon, settings, etc.).
    /// </summary>
    public event GuildUpdateEvent GuildUpdate;

    /// <summary>
    /// Delegate for GUILD_DELETE events when a guild becomes unavailable or the bot is removed.
    /// </summary>
    /// <param name="data">The guild delete data containing guild ID and unavailable status.</param>
    public delegate void GuildDeleteEvent(GuildDeleteGatewayData data);

    /// <summary>
    /// Occurs when the bot is removed from a guild or when a guild becomes unavailable.
    /// Check the Unavailable property to distinguish between guild outages (true) and the user being removed (false/null).
    /// </summary>
    public event GuildDeleteEvent GuildDelete;

    // ============================================================================
    // Guild Member Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_MEMBER_ADD events when a user joins a guild.
    /// </summary>
    /// <param name="data">The guild member data.</param>
    public delegate void GuildMemberAddEvent(GuildMemberGatewayData data);

    /// <summary>
    /// Occurs when a new member joins a guild.
    /// </summary>
    public event GuildMemberAddEvent GuildMemberAdd;

    /// <summary>
    /// Delegate for GUILD_MEMBER_UPDATE events when a guild member is updated.
    /// </summary>
    /// <param name="data">The updated guild member data.</param>
    public delegate void GuildMemberUpdateEvent(GuildMemberGatewayData data);

    /// <summary>
    /// Occurs when a guild member is updated (roles, nickname, avatar, etc.).
    /// </summary>
    public event GuildMemberUpdateEvent GuildMemberUpdate;

    /// <summary>
    /// Delegate for GUILD_MEMBER_REMOVE events when a member leaves or is removed from a guild.
    /// </summary>
    /// <param name="data">The entity data containing guild and user IDs.</param>
    public delegate void GuildMemberRemoveEvent(EntityRemovedGatewayData data);

    /// <summary>
    /// Occurs when a member leaves a guild or is kicked/banned.
    /// </summary>
    public event GuildMemberRemoveEvent GuildMemberRemove;

    // ============================================================================
    // Guild Role Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_ROLE_CREATE events when a role is created in a guild.
    /// </summary>
    /// <param name="data">The guild role data containing guild ID and role information.</param>
    public delegate void GuildRoleCreateEvent(GuildRoleGatewayData data);

    /// <summary>
    /// Occurs when a new role is created in a guild.
    /// </summary>
    public event GuildRoleCreateEvent GuildRoleCreate;

    /// <summary>
    /// Delegate for GUILD_ROLE_UPDATE events when a role is updated.
    /// </summary>
    /// <param name="data">The updated guild role data containing guild ID and role information.</param>
    public delegate void GuildRoleUpdateEvent(GuildRoleGatewayData data);

    /// <summary>
    /// Occurs when a role is updated (name, color, permissions, etc.).
    /// </summary>
    public event GuildRoleUpdateEvent GuildRoleUpdate;

    /// <summary>
    /// Delegate for GUILD_ROLE_DELETE events when a role is deleted from a guild.
    /// </summary>
    /// <param name="data">The guild role delete data containing guild and role IDs.</param>
    public delegate void GuildRoleDeleteEvent(GuildRoleDeleteGatewayData data);

    /// <summary>
    /// Occurs when a role is deleted from a guild.
    /// </summary>
    public event GuildRoleDeleteEvent GuildRoleDelete;

    // ============================================================================
    // Guild Emoji Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_EMOJIS_UPDATE events when guild emojis are updated.
    /// </summary>
    /// <param name="data">The guild emojis update data containing guild ID and updated emoji list.</param>
    public delegate void GuildEmojisUpdateEvent(GuildEmojisUpdateGatewayData data);

    /// <summary>
    /// Occurs when the list of emojis in a guild is updated (added, removed, or modified).
    /// </summary>
    public event GuildEmojisUpdateEvent GuildEmojisUpdate;

    /// <summary>
    /// Delegate for GUILD_ROLE_UPDATE_BULK events when multiple guild roles are updated.
    /// </summary>
    /// <param name="data">The guild role bulk update data containing guild ID and updated roles.</param>
    public delegate void GuildRoleUpdateBulkEvent(GuildRoleUpdateBulkGatewayData data);

    /// <summary>
    /// Occurs when multiple roles are updated at once in a guild.
    /// </summary>
    public event GuildRoleUpdateBulkEvent GuildRoleUpdateBulk;

    /// <summary>
    /// Delegate for GUILD_STICKERS_UPDATE events when guild stickers are updated.
    /// </summary>
    /// <param name="data">The guild stickers update data containing guild ID and updated sticker list.</param>
    public delegate void GuildStickersUpdateEvent(GuildStickersUpdateGatewayData data);

    /// <summary>
    /// Occurs when the list of stickers in a guild is updated (added, removed, or modified).
    /// </summary>
    public event GuildStickersUpdateEvent GuildStickersUpdate;

    // ============================================================================
    // Relationship Events
    // ============================================================================

    /// <summary>
    /// Delegate for RELATIONSHIP_ADD events when a relationship is added.
    /// </summary>
    /// <param name="data">The relationship data.</param>
    public delegate void RelationshipAddEvent(RelationshipGatewayData data);

    /// <summary>
    /// Occurs when a relationship (friend, blocked user, etc.) is added.
    /// </summary>
    public event RelationshipAddEvent RelationshipAdd;

    /// <summary>
    /// Delegate for RELATIONSHIP_UPDATE events when a relationship is updated.
    /// </summary>
    /// <param name="data">The relationship data.</param>
    public delegate void RelationshipUpdateEvent(RelationshipGatewayData data);

    /// <summary>
    /// Occurs when a relationship is updated.
    /// </summary>
    public event RelationshipUpdateEvent RelationshipUpdate;

    /// <summary>
    /// Delegate for RELATIONSHIP_REMOVE events when a relationship is removed.
    /// </summary>
    /// <param name="data">The relationship data.</param>
    public delegate void RelationshipRemoveEvent(RelationshipGatewayData data);

    /// <summary>
    /// Occurs when a relationship is removed.
    /// </summary>
    public event RelationshipRemoveEvent RelationshipRemove;

    // ============================================================================
    // Favorite Meme Events
    // ============================================================================

    /// <summary>
    /// Delegate for FAVORITE_MEME_CREATE events when a favorite meme is created.
    /// </summary>
    /// <param name="data">The favorite meme data.</param>
    public delegate void FavoriteMemeCreateEvent(FavoriteMemeGatewayData data);

    /// <summary>
    /// Occurs when a favorite meme is created.
    /// </summary>
    public event FavoriteMemeCreateEvent FavoriteMemeCreate;

    /// <summary>
    /// Delegate for FAVORITE_MEME_UPDATE events when a favorite meme is updated.
    /// </summary>
    /// <param name="data">The favorite meme data.</param>
    public delegate void FavoriteMemeUpdateEvent(FavoriteMemeGatewayData data);

    /// <summary>
    /// Occurs when a favorite meme is updated.
    /// </summary>
    public event FavoriteMemeUpdateEvent FavoriteMemeUpdate;

    /// <summary>
    /// Delegate for FAVORITE_MEME_DELETE events when a favorite meme is deleted.
    /// </summary>
    /// <param name="data">The favorite meme data.</param>
    public delegate void FavoriteMemeDeleteEvent(FavoriteMemeGatewayData data);

    /// <summary>
    /// Occurs when a favorite meme is deleted.
    /// </summary>
    public event FavoriteMemeDeleteEvent FavoriteMemeDelete;

    // ============================================================================
    // Call Events
    // ============================================================================

    /// <summary>
    /// Delegate for CALL_CREATE events when a call is created.
    /// </summary>
    /// <param name="data">The call data.</param>
    public delegate void CallCreateEvent(CallGatewayData data);

    /// <summary>
    /// Occurs when a call is created.
    /// </summary>
    public event CallCreateEvent CallCreate;

    /// <summary>
    /// Delegate for CALL_UPDATE events when a call is updated.
    /// </summary>
    /// <param name="data">The call data.</param>
    public delegate void CallUpdateEvent(CallGatewayData data);

    /// <summary>
    /// Occurs when a call is updated.
    /// </summary>
    public event CallUpdateEvent CallUpdate;

    /// <summary>
    /// Delegate for CALL_DELETE events when a call is deleted.
    /// </summary>
    /// <param name="data">The call data.</param>
    public delegate void CallDeleteEvent(CallGatewayData data);

    /// <summary>
    /// Occurs when a call is deleted.
    /// </summary>
    public event CallDeleteEvent CallDelete;

    // ============================================================================
    // Invite Events
    // ============================================================================

    /// <summary>
    /// Delegate for INVITE_CREATE events when an invite is created.
    /// </summary>
    /// <param name="data">The invite data including code, channel, guild, and inviter information.</param>
    public delegate void InviteCreateEvent(InviteGatewayData data);

    /// <summary>
    /// Occurs when a new invite is created for a guild or group DM.
    /// </summary>
    public event InviteCreateEvent InviteCreate;

    /// <summary>
    /// Delegate for INVITE_DELETE events when an invite is deleted or expires.
    /// </summary>
    /// <param name="data">The invite data containing the deleted invite code and channel/guild information.</param>
    public delegate void InviteDeleteEvent(InviteGatewayData data);

    /// <summary>
    /// Occurs when an invite is deleted or expires.
    /// </summary>
    public event InviteDeleteEvent InviteDelete;

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
                _webSocket?.Dispose();
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
