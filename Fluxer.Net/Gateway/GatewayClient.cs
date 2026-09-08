#undef NOPE
using Fluxer.Net.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Websocket.Client;

// ReSharper disable ConstantConditionalAccessQualifier
namespace Fluxer.Net.Gateway;

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
/// This client should be paired with <see cref="FluxerApiClient"/> for full Fluxer functionality.
/// </para>
/// </remarks>
public partial class FluxerGatewayClient : IDisposable
{
    #region Declares
    private FluxerClient _client;
    private WebsocketClient _webSocket;
    private readonly Stopwatch _gatewayDuration = new();

    public HashSet<ulong> GuildIds { get; private set; } = new HashSet<ulong>();

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

    private ILogger _logger;
    private CancellationTokenSource? _heartbeatCancellation;
    private readonly SemaphoreSlim _reconnectLock = new(1, 1);
    private bool _disposed = false;
    private bool _isConnecting = false;
    private int _reconnectRunning = 0;

    // build error from generated regex
    // temp. removed pending investigation.
    private static readonly Regex PacketSRegex = new(@"(?<=""s""\s*?:\s*?)\d*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

#if !NET5_0_OR_GREATER
    [ThreadStatic]
    private static Random? SharedRandom = null;
#endif

    #endregion

    #region Meta
    /// <summary>
    /// Initializes a new instance of the <see cref="FluxerGatewayClient"/> class.
    /// </summary>
    /// <remarks>
    /// The client is initialized but not connected. Call <see cref="ConnectAsync"/> to establish the gateway connection.
    /// </remarks>
    internal FluxerGatewayClient(FluxerClient client)
    {
        _client = client;
        _logger = client.Config.GatewaySerilog;
        _logger.Information("Initialized Fluxer.Net gateway client ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _client.Config.Version);
    }
    #endregion

    #region Cache

    public SocketCurrentUser? CurrentUser { get; internal set; }

    internal ulong? OwnerId { get; set; }

    public async Task<ulong?> GetOwnerIdAsync()
    {
        if (OwnerId == null)
        {
            CurrentApplication app = await _client.Rest.GetCurrentApplicationAsync();
            OwnerId = app.Owner.Id;
        }

        return OwnerId;
    }

    public ConcurrentDictionary<ulong, SocketGuild> Guilds { get; internal set; } = new ConcurrentDictionary<ulong, SocketGuild>();

    public ConcurrentDictionary<ulong, Channel> Channels { get; internal set; } = new ConcurrentDictionary<ulong, Channel>();

    public ConcurrentDictionary<ulong, SocketRole> Roles { get; internal set; } = new ConcurrentDictionary<ulong, SocketRole>();

    public ConcurrentDictionary<ulong, SocketGuildMember> CurrentMembers { get; internal set; } = new ConcurrentDictionary<ulong, SocketGuildMember>();

    public RtcRegion[] RtcRegions;

    public SocketGuildMember? GetCurrentMember(ulong guildId)
    {
        return CurrentMembers.GetValueOrDefault(guildId);
    }

    public SocketGuild? GetGuild(ulong guildId)
    {
        return Guilds.GetValueOrDefault(guildId);
    }

    public SocketRole? GetRole(ulong roleId)
    {
        return Roles.GetValueOrDefault(roleId);
    }

    public Channel? GetChannel(ulong channelId)
    {
        return Channels.GetValueOrDefault(channelId);
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

            _webSocket = new WebsocketClient(new(_client.Config.GatewayUrl))
            {
                // IMPORTANT: Do not set ReconnectTimeout here - we manage reconnection manually through the gateway protocol
                IsReconnectionEnabled = false  // Completely disable automatic reconnection
            };
            _webSocket.ReconnectionHappened.Subscribe(HandleGatewayConnect);
            _webSocket.MessageReceived.Subscribe(x => GatewayMessageHandler(x.Text));
            _webSocket.DisconnectionHappened.Subscribe(HandleGatewayDisconnect);
            Stopwatch.StartNew();

            _logger.Information("Starting WebSocket connection to {GatewayUrl}", _client.Config.GatewayUrl);
            await _webSocket.Start();

            // Wait a moment for connection to establish before sending IDENTIFY
            await Task.Delay(100);

            if (_webSocket?.IsRunning != true)
            {
                _logger.Error("WebSocket failed to start properly");
                _isConnecting = false;
                return;
            }

            GatewayPacket login = new GatewayPacket
            {
                OpCode = FluxerOpCode.Identify,
                Data = JToken.FromObject(new IdentifyGatewayData(_client.RawToken)
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "os", "linux" },
                        { "browser", "fluxer-net" },
                        { "device", "fluxer-net" }
                    },
                    IgnoredGatewayEvents = _client.Config.IgnoredGatewayEvents,
                    Presence = _client.Config.Presence
                })
            };

            SendGatewayPacket(login);
            _reconnectAttemptCount = 0; // Reset backoff counter on successful connection
        }
        finally
        {
            // Will be set to false when we receive READY event
        }
    }

    public async Task<bool> DownloadMembersAsync(SocketGuild guild)
    {
        if (guild._downloaderPromise != null)
            return await guild._downloaderPromise.Task;

        guild._downloaderPromise = new TaskCompletionSource<bool>();

        CancellationToken token = CancellationToken.None;
        GatewayPacket login = new GatewayPacket
        {
            OpCode = FluxerOpCode.RequestGuildMembers,
            Data = JToken.FromObject(new RequestMembersPacket
            {
                GuildId = guild.Id.ToString()
            })
        };
        SendGatewayPacket(login);

        await Task.WhenAny(guild._downloaderPromise.Task, Task.Delay(new TimeSpan(0, 0, 30)));

        return guild._downloaderPromise.Task.Result;
    }

    /// <summary>
    /// Sends a gateway packet to the Fluxer WebSocket server.
    /// </summary>
    /// <typeparam name="T">The type of the gateway packet data.</typeparam>
    /// <param name="Data">The packet data to serialize and send.</param>
    /// <remarks>
    /// This method automatically schedules reconnection if sending fails. Packets may be dropped
    /// during reconnection attempts. For critical operations, use the REST API via <see cref="FluxerApiClient"/>.
    /// </remarks>
    public void SendGatewayPacket<T>(T Data) where T : class
    {
        // Check if WebSocket is actually connected before trying to send
        if (_webSocket == null || !_webSocket.IsRunning)
        {
            _logger.Warning("Cannot send gateway packet - WebSocket is not connected (IsRunning={IsRunning})", _webSocket?.IsRunning ?? false);
            return;
        }

        string text = JsonConvert.SerializeObject(Data, FluxerClient._restSerializer);
        _logger.Debug("Sending serialized gateway packet {Enums}", text);
        try
        {
            _webSocket.Send(text);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to send gateway packet. Scheduling reconnection. Some packets may be dropped.");
            // Don't block - schedule reconnection on thread pool
            _ = HandleReconnectWithBackoff();
        }
    }

    /// <summary>
    /// Handles reconnection triggers
    /// </summary>
    private void TriggerReconnect()
    {
        if (Interlocked.Exchange(ref _reconnectRunning, 1) == 1)
        {
            _logger.Debug("Reconnect already scheduled");
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await HandleReconnectWithBackoff();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Reconnect loop failed");
            }
            finally
            {
                Interlocked.Exchange(ref _reconnectRunning, 0);
            }
        });
    }

    /// <summary>
    /// Handles reconnection with exponential backoff to avoid overwhelming the gateway.
    /// </summary>
    private async Task HandleReconnectWithBackoff()
    {
        _reconnectAttemptCount++;

        // Exponential backoff: 1s, 2s, 4s, 8s, 16s, max 60s
        double baseDelay = Math.Min(Math.Pow(2, _reconnectAttemptCount - 1), 60);
#if NET5_0_OR_GREATER
        double random = Random.Shared.NextDouble();
#else
        SharedRandom ??= new();
        double random = SharedRandom.NextDouble();
#endif
        double jitter = random * 0.3 * baseDelay; // Add up to 30% jitter
        TimeSpan totalDelay = TimeSpan.FromSeconds(baseDelay + jitter);

        _logger.Information("Reconnection attempt #{Attempt} - waiting {Delay:F1} seconds before reconnecting",
            _reconnectAttemptCount, totalDelay.TotalSeconds);

        await Task.Delay(totalDelay);

        try
        {
            await ReEstablishGatewayConnectionAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Reconnect attempt failed");

            // Try again
            _ = HandleReconnectWithBackoff();
        }
    }

    private void GatewayMessageHandler(string message)
    {
        // try deserializing the packet, fallback to regex
        _logger.Verbose("Received raw gateway message {Message}", message);
        try
        {
            // Pre-parse to check for InvalidSession opcode (which has a boolean payload instead of an object)
            JObject preCheck = JObject.Parse(message);
            int? opCode = preCheck["op"]?.Value<int>();
            string dispatch = preCheck["t"]?.Value<string>();

            // Log READY events specifically
            if (dispatch == "READY")
            {
                _logger.Information("Received READY event - connection fully established");
            }

            if (opCode == (int)FluxerOpCode.InvalidSession)
            {
                // InvalidSession has a boolean "d" field indicating if session is resumable
                bool canResume = preCheck["d"]?.Value<bool>() ?? false;
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
                        _webSocket?.Dispose();
                        TriggerReconnect();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Failed to reconnect after invalid session");
                    }
                });
                return;
            }

            GatewayPacket packet = JsonConvert.DeserializeObject<GatewayPacket>(message);
            if (packet == null)
            {
                _logger.Warning("Deserialized gateway packet was null");
                return;
            }

            _sequence = packet.Sequence ?? _sequence;
            //_logger.Debug("Deserialized gateway packet {@Packet}", packet);
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
                case FluxerOpCode.GatewayError:
                    _logger.Debug("Received GatewayError opcode from server");
                    return;
                case FluxerOpCode.CallConnect:
                    _logger.Debug("Received CallConnect opcode from server");
                    return;
                case FluxerOpCode.GuildSubscriptions:
                    _logger.Debug("Received GuildSubscriptions opcode from server");
                    return;
                case FluxerOpCode.RequestGuildMemberCounts:
                    _logger.Debug("Received RequestGuildMemberCounts opcode from server (unexpected)");
                    return;
                case FluxerOpCode.RequestChannelMemberCounts:
                    _logger.Debug("Received RequestChannelMemberCounts opcode from server (unexpected)");
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
                    TriggerReconnect();
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
                Match result = PacketSRegex.Match(message);
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
        //Console.WriteLine("Type: " + p.Dispatch);
        //Console.WriteLine(p.Data);
        switch (p.Dispatch)
        {
            case "READY":
                {
                    Console.WriteLine(p.Data);
                    ReadyGatewayData? data = p.Data.ToObject<ReadyGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        CurrentUser = SocketCurrentUser.Create(_client, data.User);
                        _sessionId = data.SessionId;
                        _isConnecting = false; // Connection successfully established
                        _reconnectAttemptCount = 0; // Reset backoff counter
                        Channels.Clear();
                        Roles.Clear();
                        CurrentMembers.Clear();
                        Guilds.Clear();
                        RtcRegions = data.RtcRegions.Select(x => RtcRegion.Create(_client, x)).ToArray();
                        if (data.Guilds != null)
                        {
                            GuildIds = data.Guilds.Select(x => x.Id).ToHashSet();
                            foreach (GuildGatewayData g in data.Guilds)
                            {
                                CurrentMembers.TryAdd(g.Id, SocketGuildMember.Create(_client, g.Members.First(x => x.Id == CurrentUser.Id)));
                                SocketGuild guild = SocketGuild.Create(_client, g, CurrentMembers[g.Id]);
                                foreach (GuildMemberGatewayData m in g.Members)
                                {
                                    if (m.Id != CurrentUser.Id)
                                    {
                                        SocketGuildMember member = SocketGuildMember.Create(_client, m);
                                        member.Guild = guild;
                                        guild.Members.TryAdd(m.Id, member);
                                    }
                                }

                                // Add roles
                                foreach (RoleJson r in g.Roles)
                                {
                                    SocketRole role = SocketRole.Create(_client, r, guild);
                                    Roles.TryAdd(role.Id, role);
                                    guild.Roles.TryAdd(r.Id, role);
                                    if (role.Id == guild.Id)
                                        guild.UpdatePermissions(role);
                                }

                                // Add channels
                                foreach (ChannelGatewayData c in g.Channels)
                                {
                                    Channel channel = SocketChannel.Create(_client, c, guild);
                                    if (!Channels.TryAdd(c.Id, channel))
                                    {
                                        channel = Channels[c.Id];
                                        channel.Update(c);
                                    }
                                    guild.Channels.TryAdd(c.Id, channel);
                                }

                                // Add voice states
                                foreach (VoiceStateJson v in g.VoiceStates)
                                {
                                    SocketGuildMember voiceMember = guild.AddOrUpdateMember(v.Member);
                                    SocketVoiceChannel Channel = GetChannel(v.ChannelId.Value) as SocketVoiceChannel;
                                    if (voiceMember.VoiceStates.TryGetValue(v.SessionId, out SocketVoiceState state))
                                    {
                                        state.Update(v);
                                    }
                                    else
                                    {
                                        state = SocketVoiceState.Create(_client, v, Channel);
                                        voiceMember.VoiceStates.TryAdd(v.SessionId, state);
                                        Channel.VoiceStates.TryAdd(v.SessionId, state);
                                    }
                                }


                                Guilds.TryAdd(g.Id, guild);
                                if (guild.IsAvailable)
                                    GuildAvailable?.Invoke(guild);
                            }
                        }

                        _logger.Information("Connection established successfully with session ID: {SessionId}", _sessionId);
                        Ready?.Invoke(CurrentUser);
                    }
                    else
                    {
                        _logger.Warning("READY event received but data could not be cast to ReadyGatewayData");
                    }
                }
                return;
            case "RESUMED":
                {
                    _isConnecting = false; // Connection successfully resumed
                    _reconnectAttemptCount = 0; // Reset backoff counter
                    _logger.Information("Session resumed successfully");
                    Resumed?.Invoke();
                }
                return;
            case "PASSIVE_UPDATES":
                {
                    PassiveGatewayData? data = p.Data.ToObject<PassiveGatewayData>(FluxerClient._gatewaySerializer);
                }
                return;

            // Session
            case "SESSIONS_REPLACE":
                {
                    GatewaySessionJson[]? data = p.Data.ToObject<GatewaySessionJson[]>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        SessionsReplaced?.Invoke(data.Select(x => GatewaySession.Create(_client, x)).ToArray());
                    else
                        _logger.Warning("SESSIONS_REPLACE event received but data could not be cast to SessionsReplaceGatewayData");
                }
                return;
            case "AUTH_SESSION_CHANGE":
                {
                    AuthSessionChangeGatewayData? data = p.Data.ToObject<AuthSessionChangeGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        AuthSessionChanged?.Invoke(data);
                    else
                        _logger.Warning("AUTH_SESSION_CHANGE event received but data could not be cast to AuthSessionChangeGatewayData");
                }
                return;
            case "RATE_LIMITED":
                {
                    RateLimitedGatewayData? data = p.Data.ToObject<RateLimitedGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RateLimited?.Invoke(data);
                    else
                        _logger.Warning("RATE_LIMITED event received but data could not be cast to RateLimitedGatewayData");
                }
                break;

            // User
            case "USER_UPDATE":
                {
                    CurrentUserJson? data = p.Data.ToObject<CurrentUserJson>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.Id == CurrentUser.Id)
                        {
                            SocketCurrentUser before = CurrentUser.Clone();
                            CurrentUser.Update(data);

                            CurrentUserUpdated?.Invoke(before, CurrentUser);
                        }
                    }
                    else
                        _logger.Warning("USER_UPDATE event received but data could not be cast to UserGatewayData");
                }
                return;
            case "USER_SETTINGS_UPDATE":
                {
                    UserSettingsUpdateGatewayData? data = p.Data.ToObject<UserSettingsUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        UserSettingsUpdated?.Invoke(UserSettings.Create(_client, data));
                    else
                        _logger.Warning("USER_SETTINGS_UPDATE event received but data could not be cast to UserSettingsUpdateGatewayData");
                }
                return;
            case "USER_GUILD_SETTINGS_UPDATE":
                {
                    UserGuildSettingsUpdateGatewayData? data = p.Data.ToObject<UserGuildSettingsUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        UserGuildSettingsUpdated?.Invoke(UserGuildSettings.Create(_client, data));
                    else
                        _logger.Warning("USER_GUILD_SETTINGS_UPDATE event received but data could not be cast to UserGuildSettingsUpdateGatewayData");
                }
                return;
            case "USER_NOTE_UPDATE":
                {
                    UserNoteUpdateGatewayData? data = p.Data.ToObject<UserNoteUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        UserNoteUpdated?.Invoke(data.UserId, data.Note);
                    else
                        _logger.Warning("USER_NOTE_UPDATE event received but data could not be cast to UserNoteUpdateGatewayData");
                }
                return;
            case "USER_PINNED_DMS_UPDATE":
                {
                    List<ulong>? data = p.Data.ToObject<List<ulong>>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        UserPinnedDMsUpdated?.Invoke(data);
                    else
                        _logger.Warning("USER_PINNED_DMS_UPDATE event received but data could not be cast to UserPinnedDmsUpdateGatewayData");
                }
                return;
            case "USER_CONNECTIONS_UPDATE":
                {
                    ConnectionsUpdatedGatewayData? data = p.Data.ToObject<ConnectionsUpdatedGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ConnectionsUpdated?.Invoke(data);
                    else
                        _logger.Warning("USER_CONNECTIONS_UPDATE event received but data could not be cast to ConnectionsUpdatedGatewayData");
                }
                break;
            // webauth update?

            // Relationships
            case "RELATIONSHIP_ADD":
                {
                    RelationshipGatewayData? data = p.Data.ToObject<RelationshipGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RelationshipAdded?.Invoke(data);
                    else
                        _logger.Warning("RELATIONSHIP_ADD event received but data could not be cast to RelationshipGatewayData");
                }
                return;
            case "RELATIONSHIP_UPDATE":
                {
                    RelationshipGatewayData? data = p.Data.ToObject<RelationshipGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RelationshipUpdated?.Invoke(data);
                    else
                        _logger.Warning("RELATIONSHIP_UPDATE event received but data could not be cast to RelationshipGatewayData");
                }
                return;
            case "RELATIONSHIP_REMOVE":
                {
                    RelationshipRemoveGatewayData? data = p.Data.ToObject<RelationshipRemoveGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RelationshipRemoved?.Invoke(data);
                    else
                        _logger.Warning("RELATIONSHIP_REMOVE event received but data could not be cast to RelationshipGatewayData");
                }
                return;

            // Saved messages
            case "SAVED_MESSAGE_CREATE":
                {
                    SavedMessageGatewayData? data = p.Data.ToObject<SavedMessageGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        SavedMessageCreated?.Invoke(SavedMessage.Create(_client, data));
                    else
                        _logger.Warning("SAVED_MESSAGE_CREATE event received but data could not be cast to SavedMessageGatewayData");
                }
                return;
            case "SAVED_MESSAGE_DELETE":
                {
                    SavedMessageDeletedGatewayData? data = p.Data.ToObject<SavedMessageDeletedGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        SavedMessageDeleted?.Invoke(data.MessageId);
                    else
                        _logger.Warning("SAVED_MESSAGE_DELETE event received but data could not be cast to SavedMessageGatewayData");
                }
                return;

            // Recent mentions
            case "RECENT_MENTION_DELETE":
                {
                    RecentMentionDeleteGatewayData? data = p.Data.ToObject<RecentMentionDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RecentMentionDeleted?.Invoke(data);
                    else
                        _logger.Warning("RECENT_MENTION_DELETE event received but data could not be cast to RecentMentionDeleteGatewayData");
                }
                return;

            // Favorite media
            case "FAVORITE_MEME_CREATE":
                {
                    FavoriteMediaGatewayData? data = p.Data.ToObject<FavoriteMediaGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        FavoriteMediaCreated?.Invoke(data);
                    else
                        _logger.Warning("FAVORITE_MEME_CREATE event received but data could not be cast to FavoriteMemeGatewayData");
                }
                return;
            case "FAVORITE_MEME_UPDATE":
                {
                    FavoriteMediaGatewayData? data = p.Data.ToObject<FavoriteMediaGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        FavoriteMediaUpdated?.Invoke(data);
                    else
                        _logger.Warning("FAVORITE_MEME_UPDATE event received but data could not be cast to FavoriteMemeGatewayData");
                }
                return;
            case "FAVORITE_MEME_DELETE":
                {
                    FavoriteMediaDeleteGatewayData? data = p.Data.ToObject<FavoriteMediaDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        FavoriteMediaDeleted?.Invoke(data);
                    else
                        _logger.Warning("FAVORITE_MEME_DELETE event received but data could not be cast to FavoriteMemeGatewayData");
                }
                return;

            // Guilds
            case "GUILD_CREATE":
                {
                    GuildGatewayData? data = p.Data.ToObject<GuildGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.Unavailable.HasValue && data.Unavailable.Value)
                        {
                            if (Guilds.TryGetValue(data.Id, out SocketGuild guild))
                            {
                                guild.IsAvailable = true;
                                GuildAvailable?.Invoke(guild);
                            }
                        }
                        else
                        {
                            GuildMemberGatewayData currentMemberJson = data.Members.First(x => x.Id == CurrentUser.Id);
                            SocketGuildMember currentMember = SocketGuildMember.Create(_client, currentMemberJson);

                            // Add or update current member
                            if (!CurrentMembers.TryAdd(data.Id, currentMember))
                            {
                                currentMember = CurrentMembers[data.Id];
                                currentMember.Update(currentMemberJson);
                            }

                            SocketGuild guild = SocketGuild.Create(_client, data, currentMember);

                            // Add or update guild
                            if (!Guilds.TryAdd(data.Id, guild))
                            {
                                guild = Guilds[data.Id];
                                guild.Update(data.Properties);
                            }

                            // Add or update roles
                            foreach (RoleJson r in data.Roles)
                            {
                                SocketRole role = SocketRole.Create(_client, r, guild);
                                if (!Roles.TryAdd(r.Id, role))
                                {
                                    role = Roles[r.Id];
                                    role.Update(r);
                                }

                                guild.Roles.TryAdd(r.Id, role);
                                if (role.Id == guild.Id)
                                    guild.UpdatePermissions(role);
                            }

                            // Add or update channels
                            foreach (ChannelGatewayData c in data.Channels)
                            {
                                Channel channel = SocketChannel.Create(_client, c, guild);
                                if (!Channels.TryAdd(c.Id, channel))
                                {
                                    channel = Channels[c.Id];
                                    channel.Update(c);
                                }
                                guild.Channels.TryAdd(c.Id, channel);
                            }

                            foreach (GuildMemberGatewayData m in data.Members)
                            {
                                if (m.Id != currentMember.Id)
                                    guild.AddOrUpdateMember(m);
                            }

                            // Add or update voice states
                            foreach (VoiceStateJson v in data.VoiceStates)
                            {
                                SocketGuildMember voiceMember = guild.AddOrUpdateMember(v.Member);
                                SocketVoiceChannel Channel = GetChannel(v.ChannelId.Value) as SocketVoiceChannel;
                                if (voiceMember.VoiceStates.TryGetValue(v.SessionId, out SocketVoiceState state))
                                {
                                    state.Update(v);
                                }
                                else
                                {
                                    state = SocketVoiceState.Create(_client, v, Channel);
                                    voiceMember.VoiceStates.TryAdd(v.SessionId, state);
                                    Channel.VoiceStates.TryAdd(v.SessionId, state);
                                }
                            }
                            if (guild.CurrentMember.JoinedAt > DateTime.UtcNow.AddMinutes(-1))
                                GuildJoined?.Invoke(guild);

                            GuildAvailable?.Invoke(guild);
                        }
                    }
                    else
                        _logger.Warning("GUILD_CREATE event received but data could not be cast to GuildGatewayData");
                }
                return;
            // Guild sync?
            case "GUILD_UPDATE":
                {
                    GuildJson? data = p.Data.ToObject<GuildJson>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.Id, out SocketGuild guild))
                        {
                            SocketGuild before = guild.Clone();
                            guild.Update(data);
                            GuildUpdated?.Invoke(before, guild);
                        }
                    }
                    else
                        _logger.Warning("GUILD_UPDATE event received but data could not be cast to GuildGatewayData");
                }
                return;
            case "GUILD_DELETE":
                {
                    GuildDeleteGatewayData? data = p.Data.ToObject<GuildDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.Unavailable.HasValue && data.Unavailable.Value)
                        {
                            if (Guilds.TryGetValue(data.Id, out SocketGuild guild))
                            {
                                guild.HasAllMembers = false;
                                guild.IsAvailable = false;
                                GuildUnavailable?.Invoke(guild);
                            }
                        }
                        else
                        {
                            Guilds.TryRemove(data.Id, out SocketGuild guild);
                            CurrentMembers.TryRemove(data.Id, out _);
                            foreach (Channel c in Channels.Values.Where(x => x.GuildId == data.Id))
                            {
                                Channels.TryRemove(c.Id, out _);
                            }
                            foreach (SocketRole r in Roles.Values.Where(x => x.GuildId == data.Id))
                            {
                                Roles.TryRemove(r.Id, out _);
                            }
                            GuildIds.Remove(data.Id);

                            if (guild != null)
                            {
                                GuildLeft?.Invoke(guild);
                                GuildUnavailable?.Invoke(guild);
                            }
                        }
                    }
                    else
                        _logger.Warning("GUILD_DELETE event received but data could not be cast to GuildDeleteGatewayData");
                }
                return;

            // Roles
            case "GUILD_ROLE_CREATE":
                {
                    GuildRoleGatewayData? data = p.Data.ToObject<GuildRoleGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        {
                            SocketRole role = SocketRole.Create(_client, data.Role, guild);
                            if (!Roles.TryAdd(data.Role.Id, role))
                            {
                                role = Roles[data.Role.Id];
                                role.Update(data.Role);
                            }
                            RoleCreated?.Invoke(role);
                        }
                    }
                    else
                        _logger.Warning("GUILD_ROLE_CREATE event received but data could not be cast to GuildRoleGatewayData");
                }
                return;
            case "GUILD_ROLE_UPDATE":
                {
                    GuildRoleGatewayData? data = p.Data.ToObject<GuildRoleGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            return;

                        if (!Roles.TryGetValue(data.Role.Id, out SocketRole role))
                            return;

                        SocketRole before = role.Clone();
                        role.Update(data.Role);

                        if (data.Role.Id == data.GuildId)
                            guild.UpdatePermissions(role);

                        RoleUpdated?.Invoke(before, role);
                    }
                    else
                        _logger.Warning("GUILD_ROLE_UPDATE event received but data could not be cast to GuildRoleGatewayData");
                }
                return;
            case "GUILD_ROLE_DELETE":
                {
                    GuildRoleDeleteGatewayData? data = p.Data.ToObject<GuildRoleDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            guild.Roles.TryRemove(data.RoleId, out _);

                        if (!Roles.TryRemove(data.RoleId, out SocketRole role))
                            return;

                        RoleDeleted?.Invoke(role);
                    }
                    else
                        _logger.Warning("GUILD_ROLE_DELETE event received but data could not be cast to GuildRoleDeleteGatewayData");
                }
                return;
            case "GUILD_ROLE_UPDATE_BULK":
                {
                    GuildRoleUpdateBulkGatewayData? data = p.Data.ToObject<GuildRoleUpdateBulkGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            return;

                        foreach (RoleJson r in data.Roles)
                        {
                            if (!Roles.TryGetValue(r.Id, out SocketRole role))
                                continue;

                            SocketRole before = role.Clone();
                            role.Update(r);

                            if (r.Id == data.GuildId)
                                guild.UpdatePermissions(role);

                            RoleUpdated?.Invoke(before, role);
                        }
                    }
                    else
                        _logger.Warning("GUILD_ROLE_UPDATE_BULK event received but data could not be cast to GuildRoleUpdateBulkGatewayData");
                }
                return;

            // Expressions
            case "GUILD_EMOJIS_UPDATE":
                {
                    GuildEmojisUpdateGatewayData? data = p.Data.ToObject<GuildEmojisUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            return;

                        IEnumerable<GuildEmojiJson> newEmojis = data.Emojis.Where(x => !guild.Emojis.ContainsKey(x.Id));
                        IEnumerable<KeyValuePair<ulong, GuildEmoji>> deletedEmojis = guild.Emojis.Where(x => !data.Emojis.Any(y => y.Id == x.Key));
                        (GuildEmoji Entity, GuildEmojiJson Model)[] updatedEmojis = data.Emojis.Select(x =>
                        {
                            GuildEmoji s = guild.Emojis.GetValueOrDefault(x.Id);
                            if (s == null)
                                return null;

                            var e = s.Compare(x);
                            if (!e)
                            {
                                return (s, x) as (GuildEmoji Entity, GuildEmojiJson Model)?;
                            }
                            else
                            {
                                return null;
                            }
                        }).Where(x => x.HasValue).Select(x => x.Value).ToArray();

                        foreach (GuildEmojiJson model in newEmojis)
                        {
                            GuildEmoji emoji = GuildEmoji.Create(_client, model, guild.Id);
                            guild.Emojis.TryAdd(emoji.Id, emoji);
                            EmojiCreated?.Invoke(emoji);
                        }
                        foreach (KeyValuePair<ulong, GuildEmoji> emoji in deletedEmojis)
                        {
                            guild.Emojis.TryRemove(emoji.Key, out _);
                            EmojiDeleted?.Invoke(emoji.Value);
                        }
                        foreach ((GuildEmoji Entity, GuildEmojiJson Model) entityModelPair in updatedEmojis)
                        {
                            GuildEmoji before = entityModelPair.Entity.Clone();

                            entityModelPair.Entity.Update(entityModelPair.Model);

                            EmojiUpdated?.Invoke(before, entityModelPair.Entity);
                        }
                    }
                    else
                        _logger.Warning("GUILD_EMOJIS_UPDATE event received but data could not be cast to GuildEmojisUpdateGatewayData");
                }
                return;
            case "GUILD_STICKERS_UPDATE":
                {
                    GuildStickersUpdateGatewayData? data = p.Data.ToObject<GuildStickersUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            return;

                        IEnumerable<GuildStickerJson> newStickers = data.Stickers.Where(x => !guild.Stickers.ContainsKey(x.Id));
                        IEnumerable<KeyValuePair<ulong, GuildSticker>> deletedStickers = guild.Stickers.Where(x => !data.Stickers.Any(y => y.Id == x.Key));
                        (GuildSticker Entity, GuildStickerJson Model)[] updatedStickers = data.Stickers.Select(x =>
                        {
                            GuildSticker s = guild.Stickers.GetValueOrDefault(x.Id);
                            if (s == null)
                                return null;

                            var e = s.Compare(x);
                            if (!e)
                            {
                                return (s, x) as (GuildSticker Entity, GuildStickerJson Model)?;
                            }
                            else
                            {
                                return null;
                            }
                        }).Where(x => x.HasValue).Select(x => x.Value).ToArray();

                        foreach (GuildStickerJson model in newStickers)
                        {
                            GuildSticker sticker = GuildSticker.Create(_client, model, guild.Id);
                            guild.Stickers.TryAdd(sticker.Id, sticker);
                            StickerCreated?.Invoke(sticker);
                        }
                        foreach (KeyValuePair<ulong, GuildSticker> sticker in deletedStickers)
                        {
                            guild.Stickers.TryRemove(sticker.Key, out _);
                            StickerDeleted?.Invoke(sticker.Value);
                        }
                        foreach ((GuildSticker Entity, GuildStickerJson Model) entityModelPair in updatedStickers)
                        {
                            GuildSticker before = entityModelPair.Entity.Clone();

                            entityModelPair.Entity.Update(entityModelPair.Model);

                            StickerUpdated?.Invoke(before, entityModelPair.Entity);
                        }
                    }
                    else
                        _logger.Warning("GUILD_STICKERS_UPDATE event received but data could not be cast to GuildStickersUpdateGatewayData");
                }
                return;

            // Channels
            case "CHANNEL_CREATE":
                {
                    ChannelGatewayData? data = p.Data.ToObject<ChannelGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        SocketGuild? guild = data.GuildId.HasValue ? GetGuild(data.GuildId.Value) : null;
                        Channel channel = SocketChannel.Create(_client, data, guild);
                        if (!Channels.TryAdd(channel.Id, channel))
                        {
                            channel = Channels[channel.Id];
                            channel.Update(data);
                        }
                        if (guild != null)
                            guild.Channels.TryAdd(channel.Id, channel);

                        ChannelCreated?.Invoke(channel);
                    }
                    else
                        _logger.Warning("CHANNEL_CREATE event received but data could not be cast to ChannelGatewayData");
                }
                return;
            case "CHANNEL_UPDATE":
                {
                    ChannelGatewayData? data = p.Data.ToObject<ChannelGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.Id, out Channel channel))
                        {
                            Channel before = channel.Clone();
                            channel.Update(data);
                            ChannelUpdated?.Invoke(before, channel);
                        }
                        else
                        {
                            channel = SocketChannel.Create(_client, data);
                            Channels.TryAdd(data.Id, channel);
                            if (channel.GuildId.HasValue && Guilds.TryGetValue(channel.GuildId.Value, out SocketGuild guild))
                                guild.Channels.TryAdd(data.Id, channel);
                        }
                    }
                    else
                        _logger.Warning("CHANNEL_UPDATE event received but data could not be cast to ChannelGatewayData");
                }
                return;
            case "CHANNEL_UPDATE_BULK":
                {
                    ChannelUpdateBulkGatewayData? data = p.Data.ToObject<ChannelUpdateBulkGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        foreach (ChannelJson c in data.Channels)
                        {
                            if (Channels.TryGetValue(c.Id, out Channel channel))
                            {
                                Channel before = channel.Clone();
                                channel.Update(c);
                                ChannelUpdated?.Invoke(before, channel);
                            }
                        }
                    }
                    else
                        _logger.Warning("CHANNEL_UPDATE_BULK event received but data could not be cast to ChannelUpdateBulkGatewayData");
                }
                return;
            case "CHANNEL_DELETE":
                {
                    ChannelGatewayData? data = p.Data.ToObject<ChannelGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        Channels.TryRemove(data.Id, out Channel channel);
                        ChannelDeleted?.Invoke(channel ?? SocketChannel.Create(_client, data));
                    }
                    else
                        _logger.Warning("CHANNEL_DELETE event received but data could not be cast to ChannelGatewayData");
                }
                return;

            // Groups
            case "CHANNEL_RECIPIENT_ADD":
                {
                    ChannelRecipientGatewayData? data = p.Data.ToObject<ChannelRecipientGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            GroupUserAdded?.Invoke(channel, SocketUser.Create(_client, data.User));
                    }
                    else
                        _logger.Warning("CHANNEL_RECIPIENT_ADD event received but data could not be cast to ChannelRecipientGatewayData");
                }
                return;
            case "CHANNEL_RECIPIENT_REMOVE":
                {
                    ChannelRecipientGatewayData? data = p.Data.ToObject<ChannelRecipientGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            GroupUserRemoved?.Invoke(channel, SocketUser.Create(_client, data.User));
                    }
                    else
                        _logger.Warning("CHANNEL_RECIPIENT_REMOVE event received but data could not be cast to ChannelRecipientGatewayData");
                }
                return;

            // Webhooks
            case "WEBHOOKS_UPDATE":
                {
                    WebhooksUpdateGatewayData? data = p.Data.ToObject<WebhooksUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        SocketGuild guild = GetGuild(data.GuildId);
                        Channel channel = GetChannel(data.ChannelId);
                        if (guild == null || channel == null)
                            return;

                        WebhooksUpdated?.Invoke(guild, channel);
                    }
                    else
                        _logger.Warning("WEBHOOKS_UPDATE event received but data could not be cast to WebhooksUpdateGatewayData");
                }
                return;

            // Invites
            case "INVITE_CREATE":
                {
                    InviteJson? data = p.Data.ToObject<InviteJson>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        InviteCreated?.Invoke(Invite.Create(_client, data));
                    }
                    else
                        _logger.Warning("INVITE_CREATE event received but data could not be cast to InviteGatewayData");
                }
                return;
            case "INVITE_DELETE":
                {
                    InviteDeleteGatewayData? data = p.Data.ToObject<InviteDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        InviteDeleted?.Invoke(data);
                    else
                        _logger.Warning("INVITE_DELETE event received but data could not be cast to InviteGatewayData");
                }
                return;

            // Members
            case "GUILD_MEMBER_ADD":
                {
                    GuildMemberGatewayData? data = p.Data.ToObject<GuildMemberGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        {
                            SocketGuildMember member = guild.AddOrUpdateMember(data);
                            MemberJoined?.Invoke(member);
                        }
                    }
                    else
                        _logger.Warning("GUILD_MEMBER_ADD event received but data could not be cast to GuildMemberGatewayData");
                }
                return;
            case "GUILD_MEMBER_UPDATE":
                {
                    GuildMemberGatewayData? data = p.Data.ToObject<GuildMemberGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        {
                            SocketGuildMember member = guild.AddOrUpdateMember(data);
                            MemberUpdated?.Invoke(member);
                        }
                    }
                    else
                        _logger.Warning("GUILD_MEMBER_UPDATE event received but data could not be cast to GuildMemberGatewayData");
                }
                return;
            case "GUILD_MEMBER_REMOVE":
                {
                    GuildMemberRemoveGatewayData? data = p.Data.ToObject<GuildMemberRemoveGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        ulong userId = data.User.Id;
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        {
                            guild.Members.TryRemove(userId, out SocketGuildMember? member);
                            MemberLeft?.Invoke(guild, new Cacheable<SocketGuildMember>(userId, member, () =>
                            {
                                return null;
                            }));
                        }
                    }
                    else
                        _logger.Warning("GUILD_MEMBER_REMOVE event received but data could not be cast to EntityRemovedGatewayData");
                }
                return;
            case "GUILD_MEMBERS_CHUNK":
                {
                    GuildMembersChunkGatewayData? data = p.Data.ToObject<GuildMembersChunkGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        {
                            foreach (GuildMemberJson m in data.Members)
                            {
                                guild.AddOrUpdateMember(m);
                            }
                            if ((data.ChunkIndex + 1) == data.ChunkCount)
                            {
                                guild.HasAllMembers = true;
                                if (guild._downloaderPromise != null)
                                    guild._downloaderPromise.SetResult(true);
                            }
                        }
                    }
                    else
                        _logger.Warning("GUILD_MEMBERS_CHUNK event received but data could not be cast to GuildMembersChunkGatewayData");
                }
                return;

            // Audit log
            case "GUILD_AUDIT_LOG_ENTRY_CREATE":
                {
                    //TODO
                }
                break;

            // Bans
            case "GUILD_BAN_ADD":
                {
                    GuildBanGatewayData? data = p.Data.ToObject<GuildBanGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out var guild))
                            UserBanned?.Invoke(guild, new Cacheable<SocketGuildMember>(data.User.Id, guild.GetMember(data.User.Id), () =>
                            {
                                return null;
                            }));
                    }
                    else
                        _logger.Warning("GUILD_BAN_ADD event received but data could not be cast to GuildBanGatewayData");
                }
                return;
            case "GUILD_BAN_REMOVE":
                {
                    GuildBanGatewayData? data = p.Data.ToObject<GuildBanGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out var guild))
                            UserUnbanned?.Invoke(guild, data.User.Id);
                    }
                    else
                        _logger.Warning("GUILD_BAN_REMOVE event received but data could not be cast to GuildBanGatewayData");
                }
                return;

            // Presence
            case "PRESENCE_UPDATE":
                {
                    PresenceGatewayData? data = p.Data.ToObject<PresenceGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        PresenceUpdated?.Invoke(data);
                    else
                        _logger.Warning("PRESENCE_UPDATE event received but data could not be cast to PresenceGatewayData");
                }
                return;
            case "PRESENCE_UPDATE_BULK":
                {
                    PresenceUpdatedBulkGatewayData? data = p.Data.ToObject<PresenceUpdatedBulkGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        foreach (PresenceGatewayData i in data.Presences)
                        {
                            PresenceUpdated?.Invoke(i);
                        }
                    }
                    else
                        _logger.Warning("PRESENCE_UPDATE event received but data could not be cast to PresenceUpdatedBulkGatewayData");
                }
                break;

            // Passive updates?

            // Messages
            case "MESSAGE_CREATE":
                {
                    MessageGatewayData? data = p.Data.ToObject<MessageGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        MessageCreated?.Invoke(SocketMessage.Create(_client, data));
                    else
                        _logger.Warning("MESSAGE_CREATE event received but data could not be cast to MessageGatewayData");
                }
                return;
            case "MESSAGE_UPDATE":
                {
                    MessageGatewayData? data = p.Data.ToObject<MessageGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.Member != null)
                            data.Member.User = data.Author;

                        MessageUpdated?.Invoke(SocketMessage.Create(_client, data));
                    }
                    else
                        _logger.Warning("MESSAGE_UPDATE event received but data could not be cast to MessageGatewayData");
                }
                return;
            case "MESSAGE_DELETE":
                {
                    MessageDeleteGatewayData? data = p.Data.ToObject<MessageDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            MessageDeleted?.Invoke(channel, data.AuthorId, data.MessageId, data.Content);
                    }
                    else
                        _logger.Warning("MESSAGE_DELETE event received but data could not be cast to EntityRemovedGatewayData");
                }
                return;
            case "MESSAGE_DELETE_BULK":
                {
                    MessageBulkDeleteGatewayData? data = p.Data.ToObject<MessageBulkDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        MessagesDeleted?.Invoke(data);
                    else
                        _logger.Warning("MESSAGE_DELETE_BULK event received but data could not be cast to MessageBulkDeleteGatewayData");
                }
                return;
            case "MESSAGE_ACK":
                {
                    MessageAckGatewayData? data = p.Data.ToObject<MessageAckGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        MessageAck?.Invoke(data);
                    else
                        _logger.Warning("MESSAGE_ACK event received but data could not be cast to MessageAckGatewayData");
                }
                return;

            // Reactions
            case "MESSAGE_REACTION_ADD":
                {
                    MessageReactionGatewayData? data = p.Data.ToObject<MessageReactionGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        MessageReactionAdded?.Invoke(data);
                    }
                    else
                        _logger.Warning("MESSAGE_REACTION_ADD event received but data could not be cast to MessageReactionGatewayData");
                }
                return;
            case "MESSAGE_REACTION_REMOVE":
                {
                    MessageReactionGatewayData? data = p.Data.ToObject<MessageReactionGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        MessageReactionRemoved?.Invoke(data);
                    else
                        _logger.Warning("MESSAGE_REACTION_REMOVE event received but data could not be cast to MessageReactionGatewayData");
                }
                return;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    MessageReactionsRemoveGatewayData? data = p.Data.ToObject<MessageReactionsRemoveGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        MessageReactionRemoveAll?.Invoke(data);
                    else
                        _logger.Warning("MESSAGE_REACTION_REMOVE_ALL event received but data could not be cast to MessageReactionsRemovedGatewayData");
                }
                return;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    MessageReactionRemoveEmojiGatewayData? data = p.Data.ToObject<MessageReactionRemoveEmojiGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        MessageReactionRemovedEmoji?.Invoke(data);
                    else
                        _logger.Warning("MESSAGE_REACTION_REMOVE_EMOJI event received but data could not be cast to MessageReactionRemoveEmojiGatewayData");
                }
                return;

            // Typing
            case "TYPING_START":
                {
                    TypingGatewayData? data = p.Data.ToObject<TypingGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        TypingStarted?.Invoke(data);
                    else
                        _logger.Warning("TYPING_START event received but data could not be cast to TypingGatewayData");
                }
                return;

            // Pins
            case "CHANNEL_PINS_UPDATE":
                {
                    ChannelPinsUpdateGatewayData? data = p.Data.ToObject<ChannelPinsUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ChannelPinsUpdated?.Invoke(data);
                    else
                        _logger.Warning("CHANNEL_PINS_UPDATE event received but data could not be cast to ChannelPinsUpdateGatewayData");
                }
                return;
            case "CHANNEL_PINS_ACK":
                {
                    ChannelPinsAckGatewayData? data = p.Data.ToObject<ChannelPinsAckGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ChannelPinsAck?.Invoke(data);
                    else
                        _logger.Warning("CHANNEL_PINS_ACK event received but data could not be cast to ChannelPinsAckGatewayData");
                }
                return;

            // Voice
            // Voice state ack?
            case "VOICE_STATE_UPDATE":
                {
                    VoiceStateGatewayData? data = p.Data.ToObject<VoiceStateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        Channel? channel = data.ChannelId.HasValue ? GetChannel(data.ChannelId.Value) : null;

                        SocketVoiceState currentState = SocketVoiceState.Create(_client, data, channel);
                        if (data.GuildId.HasValue && Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                        {
                            guild.AddOrUpdateMember(data.Member);
                            SocketGuildMember member = guild.GetMember(data.Member.Id);
                            if (data.ChannelId.HasValue)
                            {
                                SocketVoiceChannel? Channel = GetChannel(data.ChannelId.Value) as SocketVoiceChannel;
                                if (member.VoiceStates.TryGetValue(data.SessionId, out SocketVoiceState state))
                                {
                                    state.Update(data);
                                }
                                else
                                {
                                    state = currentState;
                                    member.VoiceStates.TryAdd(data.SessionId, state);
                                    if (Channel != null)
                                        Channel.VoiceStates.TryAdd(data.SessionId, state);
                                }
                            }
                            else
                            {
                                if (member.VoiceStates.TryRemove(data.SessionId, out SocketVoiceState oldState))
                                {
                                    SocketVoiceChannel? Channel = GetChannel(oldState.ChannelId.Value) as SocketVoiceChannel;
                                    if (Channel != null)
                                        Channel.VoiceStates.TryRemove(data.SessionId, out _);
                                }
                            }
                        }

                        VoiceStateUpdated?.Invoke(currentState);
                    }
                    else
                        _logger.Warning("VOICE_STATE_UPDATE event received but data could not be cast to VoiceStateGatewayData");
                }
                return;
            case "VOICE_SERVER_UPDATE":
                {
                    VoiceServerUpdateGatewayData? data = p.Data.ToObject<VoiceServerUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        VoiceServerUpdated?.Invoke(VoiceServer.Create(_client, data));
                    else
                        _logger.Warning("VOICE_SERVER_UPDATE event received but data could not be cast to VoiceServerUpdateGatewayData");
                }
                return;
            // Entrance sound

            // Calls
            case "CALL_CREATE":
                {
                    CallCreateGatewayData? data = p.Data.ToObject<CallCreateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        CallCreated?.Invoke(data);
                    else
                        _logger.Warning("CALL_CREATE event received but data could not be cast to CallGatewayData");
                }
                return;
            case "CALL_UPDATE":
                {
                    CallUpdateGatewayData? data = p.Data.ToObject<CallUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        CallUpdated?.Invoke(data);
                    else
                        _logger.Warning("CALL_UPDATE event received but data could not be cast to CallGatewayData");
                }
                return;
            case "CALL_DELETE":
                {
                    CallDeleteGatewayData? data = p.Data.ToObject<CallDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        CallDeleted?.Invoke(data);
                    else
                        _logger.Warning("CALL_DELETE event received but data could not be cast to CallGatewayData");
                }
                return;

            // Counts
            case "GUILD_COUNTS_UPDATE":
                {
                    CountGatewayData<GuildMemberCountGatewayData> data = p.Data.ToObject<CountGatewayData<GuildMemberCountGatewayData>>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        GuildMemberCounts?.Invoke(data);
                    else
                        _logger.Warning("GUILD_COUNTS_UPDATE event received but data could not be cast to GuildMemberCountGatewayData");
                }
                break;
            case "CHANNEL_MEMBER_COUNTS_UPDATE":
                {
                    CountGatewayData<GuildChannelMemberCountGatewayData> data = p.Data.ToObject<CountGatewayData<GuildChannelMemberCountGatewayData>>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ChannelMemberCounts?.Invoke(data);
                    else
                        _logger.Warning("CHANNEL_MEMBER_COUNTS_UPDATE event received but data could not be cast to GuildChannelMemberCountGatewayData");
                }
                break;

            default:
                _logger.Debug(p.Data.ToString());
                _logger.Warning("Unhandled dispatch {Dispatch}", p.Dispatch);
                break;
        }
    }

    private void HandleHello(GatewayPacket packet)
    {
        HelloGatewayData? data = packet.Data.ToObject<HelloGatewayData>(FluxerClient._gatewaySerializer);
        if (data == null)
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

        if (!await _reconnectLock.WaitAsync(0))
        {
            _logger.Debug("Reconnection already in progress, skipping duplicate attempt");
            return;
        }

        try
        {
            TimeSpan timeSinceLastAttempt = DateTime.Now - _lastGatewayReEstablishAttempt;
            TimeSpan requiredDelay = TimeSpan.FromSeconds(_client.Config.ReconnectAttemptDelay);

            if (timeSinceLastAttempt < requiredDelay)
            {
                TimeSpan remainingDelay = requiredDelay - timeSinceLastAttempt;

                _logger.Warning(
                    "Rate limiting reconnection. Waiting {RemainingSeconds:F1} seconds before reconnect attempt.",
                    remainingDelay.TotalSeconds);

                await Task.Delay(remainingDelay);
            }

            _lastGatewayReEstablishAttempt = DateTime.Now;

            // Kill old transport
            if (_webSocket != null)
            {
                try
                {
                    _webSocket.Dispose();
                    _logger.Debug("Disposed old gateway WebSocket");
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Failed disposing old WebSocket");
                }
            }

            // Create NEW transport
            _webSocket = new WebsocketClient(new(_client.Config.GatewayUrl))
            {
                IsReconnectionEnabled = false
            };

            _webSocket.ReconnectionHappened.Subscribe(HandleGatewayConnect);
            _webSocket.MessageReceived.Subscribe(x => GatewayMessageHandler(x.Text));
            _webSocket.DisconnectionHappened.Subscribe(HandleGatewayDisconnect);

            _logger.Information(
                "Starting new WebSocket connection to {GatewayUrl}",
                _client.Config.GatewayUrl);

            await _webSocket.Start();

            await Task.Delay(100);

            if (_webSocket.IsRunning != true)
            {
                throw new Exception("New gateway WebSocket failed to start");
            }

            GatewayPacket packet;

            if (!string.IsNullOrEmpty(_sessionId))
            {
                _logger.Information(
                    "Attempting to resume session {SessionId} with sequence {Sequence}",
                    _sessionId,
                    _sequence);

                packet = new GatewayPacket()
                {
                    OpCode = FluxerOpCode.Resume,
                    Data = JToken.FromObject(new ReconnectGatewayData()
                    {
                        Sequence = _sequence,
                        SessionId = _sessionId,
                        Token = _client.RawToken
                    })
                };
            }
            else
            {
                _logger.Information("No session ID, sending IDENTIFY");

                packet = new GatewayPacket
                {
                    OpCode = FluxerOpCode.Identify,
                    Data = JToken.FromObject(new IdentifyGatewayData(_client.RawToken)
                    {
                        Properties = new Dictionary<string, string>
                        {
                            { "os", Environment.OSVersion.Platform.ToString() },
                            { "browser", "Fluxer.Net" },
                            { "device", "Fluxer.Net" }
                        },
                        IgnoredGatewayEvents = _client.Config.IgnoredGatewayEvents,
                        Presence = _client.Config.Presence
                    })
                };
            }

            SendGatewayPacket(packet);
        }
        finally
        {
            _reconnectLock.Release();
        }
    }

    private void HandleGatewayConnect(ReconnectionInfo info)
    {
        _logger.Information($"WebSocket connected: Type={info.Type}");
        Connected?.Invoke();
    }

    private void HandleGatewayDisconnect(DisconnectionInfo info)
    {
        _isConnecting = false;

        _logger.Warning(
            "WebSocket disconnected: Type={Type}, CloseStatus={CloseStatus}, CloseStatusDescription={CloseDescription}, Exception={Exception}",
            info.Type,
            info.CloseStatus?.ToString() ?? "None",
            info.CloseStatusDescription ?? "None",
            info.Exception?.Message ?? "None");

        bool shouldReconnect = true;

        if (info.CloseStatus.HasValue)
        {
            shouldReconnect = HandleGatewayCloseCode(
                (int)info.CloseStatus.Value,
                info.CloseStatusDescription);
        }

        if (shouldReconnect &&
            (info.Type == DisconnectionType.ByServer ||
             info.Type == DisconnectionType.Lost ||
             info.Type == DisconnectionType.Error))
        {
            _logger.Information("Connection lost, manually reconnecting...");

            TriggerReconnect();
        }
        else if (!shouldReconnect)
        {
            _logger.Warning(
                "Reconnection disabled due to close code. Manual intervention required.");

            _reconnectAttemptCount = 0;
        }
        Disconnected?.Invoke();
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

        FluxerCloseCode fluxerCode = (FluxerCloseCode)closeCode;
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
#if NET5_0_OR_GREATER
        int jitter = Random.Shared.Next(0, 500);
#else
        SharedRandom ??= new();
        int jitter = SharedRandom.Next(0, 500);
#endif

        _logger.Debug("Starting heartbeat with interval {Interval}ms and jitter {Jitter}ms", _heartbeatInterval, jitter);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_heartbeatInterval + jitter, cancellationToken);

                _logger.Verbose("Sending heartbeat with sequence {Sequence}", _sequence);
                HeartbeatPacket packet = new HeartbeatPacket()
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
    public void SetStatus(Status status, UserCustomStatusJson? custom)
    {
        GatewayPacket packet = new GatewayPacket()
        {
            Data = JToken.FromObject(new PresenceUpdateGatewayData(status, custom)),
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
    /// Delegate for gateway connected.
    /// </summary>
    public delegate void ConnectedEvent();

    /// <summary>
    /// Occurs when the gateway is connected.
    /// </summary>
    public event ConnectedEvent Connected;

    /// <summary>
    /// Delegate for gateway disconnected.
    /// </summary>
    public delegate void DisconnectedEvent();

    /// <summary>
    /// Occurs when the gateway is disconnected.
    /// </summary>
    public event DisconnectedEvent Disconnected;

    /// <summary>
    /// Delegate for the READY event fired when the gateway connection is established.
    /// </summary>
    /// <param name="currentUser">The current user of the client.</param>
    public delegate void ReadyEvent(SocketCurrentUser currentUser);

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
    public delegate void SessionsReplacedEvent(GatewaySession[] sessions);

    /// <summary>
    /// Occurs when auth sessions are replaced.
    /// </summary>
    public event SessionsReplacedEvent SessionsReplaced;

    // ============================================================================
    // User Settings Events
    // ============================================================================

    /// <summary>
    /// Delegate for USER_SETTINGS_UPDATE events when user settings are updated.
    /// </summary>
    /// <param name="data">The user settings data.</param>
    public delegate void UserSettingsUpdatedEvent(UserSettings data);

    /// <summary>
    /// Occurs when user settings are updated.
    /// </summary>
    public event UserSettingsUpdatedEvent UserSettingsUpdated;

    /// <summary>
    /// Delegate for USER_GUILD_SETTINGS_UPDATE events when user guild settings are updated.
    /// </summary>
    /// <param name="data">The user guild settings data.</param>
    public delegate void UserGuildSettingsUpdatedEvent(UserGuildSettings data);

    /// <summary>
    /// Occurs when user guild settings are updated.
    /// </summary>
    public event UserGuildSettingsUpdatedEvent UserGuildSettingsUpdated;

    /// <summary>
    /// Delegate for USER_PINNED_DMS_UPDATE events when pinned DMs are updated.
    /// </summary>
    /// <param name="data">The pinned DMs data.</param>
    public delegate void UserPinnedDMsUpdatedEvent(List<ulong> data);

    /// <summary>
    /// Occurs when pinned DMs are updated.
    /// </summary>
    public event UserPinnedDMsUpdatedEvent UserPinnedDMsUpdated;

    /// <summary>
    /// Delegate for USER_NOTE_UPDATE events when a user note is updated.
    /// </summary>
    /// <param name="data">The user note data.</param>
    public delegate void UserNoteUpdatedEvent(ulong userId, string note);

    /// <summary>
    /// Occurs when a user note is updated.
    /// </summary>
    public event UserNoteUpdatedEvent UserNoteUpdated;

    /// <summary>
    /// Delegate for AUTH_SESSION_CHANGE events when an auth session changes.
    /// </summary>
    /// <param name="data">The auth session data.</param>
    public delegate void AuthSessionChangedEvent(AuthSessionChangeGatewayData data);

    /// <summary>
    /// Occurs when an auth session changes.
    /// </summary>
    public event AuthSessionChangedEvent AuthSessionChanged;

    /// <summary>
    /// Delegate for RATE_LIMITED event when rate limited.
    /// </summary>
    public delegate void RateLimitedEvent(RateLimitedGatewayData data);

    /// <summary>
    /// Occurs on rate limited.
    /// </summary>
    public event RateLimitedEvent RateLimited;

    /// <summary>
    /// Delegate for USER_CONNECTIONS_UPDATE event when user updates a connection.
    /// </summary>
    public delegate void UserConnectionsUpdatedEvent(ConnectionsUpdatedGatewayData data);

    /// <summary>
    /// Occurs on user connection updated.
    /// </summary>
    public event UserConnectionsUpdatedEvent ConnectionsUpdated;

    // ============================================================================
    // Message Events
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_CREATE events when a new message is sent.
    /// </summary>
    /// <param name="data">The message data including content, author, channel, etc.</param>
    public delegate void MessageCreatedEvent(SocketMessage data);

    /// <summary>
    /// Occurs when a new message is created in any channel the user has access to.
    /// </summary>
    public event MessageCreatedEvent MessageCreated;

    /// <summary>
    /// Delegate for MESSAGE_UPDATE events when a message is edited.
    /// </summary>
    /// <param name="data">The updated message data.</param>
    public delegate void MessageUpdatedEvent(SocketMessage data);

    /// <summary>
    /// Occurs when a message is edited.
    /// </summary>
    public event MessageUpdatedEvent MessageUpdated;

    /// <summary>
    /// Delegate for MESSAGE_DELETE events when a message is deleted.
    /// </summary>
    public delegate void MessageDeletedEvent(Channel channel, ulong? userId, ulong messageId, string? content);

    /// <summary>
    /// Occurs when a message is deleted.
    /// </summary>
    public event MessageDeletedEvent MessageDeleted;

    // ============================================================================
    // Channel Events
    // ============================================================================

    /// <summary>
    /// Delegate for CHANNEL_CREATE events when a channel is created.
    /// </summary>
    /// <param name="data">The channel data.</param>
    public delegate void ChannelCreatedEvent(Channel data);

    /// <summary>
    /// Occurs when a channel is created in a guild.
    /// </summary>
    public event ChannelCreatedEvent ChannelCreated;

    /// <summary>
    /// Delegate for CHANNEL_UPDATE events when a channel is updated.
    /// </summary>
    public delegate void ChannelUpdatedEvent(Channel before, Channel after);

    /// <summary>
    /// Occurs when a channel is updated.
    /// </summary>
    public event ChannelUpdatedEvent ChannelUpdated;

    /// <summary>
    /// Delegate for CHANNEL_DELETE events when a channel is deleted.
    /// </summary>
    /// <param name="data">The deleted channel data.</param>
    public delegate void ChannelDeletedEvent(Channel data);

    /// <summary>
    /// Occurs when a channel is deleted.
    /// </summary>
    public event ChannelDeletedEvent ChannelDeleted;

    // ============================================================================
    // User Events
    // ============================================================================

    /// <summary>
    /// Delegate for USER_UPDATE events when the current user's data is updated.
    /// </summary>
    public delegate void CurrentUserUpdatedEvent(SocketCurrentUser before, SocketCurrentUser after);

    /// <summary>
    /// Occurs when the current user's account is updated.
    /// </summary>
    public event CurrentUserUpdatedEvent CurrentUserUpdated;

    // ============================================================================
    // Presence Events
    // ============================================================================

    /// <summary>
    /// Delegate for PRESENCE_UPDATE events when a user's presence changes.
    /// </summary>
    /// <param name="data">The presence data.</param>
    public delegate void PresenceUpdatedEvent(PresenceGatewayData data);

    /// <summary>
    /// Occurs when a user's presence (online status) changes.
    /// </summary>
    public event PresenceUpdatedEvent PresenceUpdated;

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
    public event TypingStartEvent TypingStarted;

    // ============================================================================
    // Message Reaction Events
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_REACTION_ADD events when a reaction is added.
    /// </summary>
    /// <param name="data">The reaction data.</param>
    public delegate void MessageReactionAddedEvent(MessageReactionGatewayData data);

    /// <summary>
    /// Occurs when a user adds a reaction to a message.
    /// </summary>
    public event MessageReactionAddedEvent MessageReactionAdded;

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE events when a reaction is removed.
    /// </summary>
    /// <param name="data">The reaction data.</param>
    public delegate void MessageReactionRemovedEvent(MessageReactionGatewayData data);

    /// <summary>
    /// Occurs when a user removes a reaction from a message.
    /// </summary>
    public event MessageReactionRemovedEvent MessageReactionRemoved;

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_ALL events when all reactions are cleared.
    /// </summary>
    public delegate void MessageReactionRemoveAllEvent(MessageReactionsRemoveGatewayData data);

    /// <summary>
    /// Occurs when all reactions are removed from a message.
    /// </summary>
    public event MessageReactionRemoveAllEvent MessageReactionRemoveAll;

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_EMOJI events when all reactions of a specific emoji are removed.
    /// </summary>
    /// <param name="data">The reaction removal data.</param>
    public delegate void MessageReactionRemovedEmojiEvent(MessageReactionRemoveEmojiGatewayData data);

    /// <summary>
    /// Occurs when all instances of a specific emoji are removed from a message.
    /// </summary>
    public event MessageReactionRemovedEmojiEvent MessageReactionRemovedEmoji;

    // ============================================================================
    // Saved Messages and Mentions
    // ============================================================================

    /// <summary>
    /// Delegate for SAVED_MESSAGE_CREATE events when a message is saved.
    /// </summary>
    /// <param name="data">The saved message data.</param>
    public delegate void SavedMessageCreatedEvent(SavedMessage data);

    /// <summary>
    /// Occurs when a message is saved.
    /// </summary>
    public event SavedMessageCreatedEvent SavedMessageCreated;

    /// <summary>
    /// Delegate for SAVED_MESSAGE_DELETE events when a saved message is deleted.
    /// </summary>
    public delegate void SavedMessageDeletedEvent(ulong messageId);

    /// <summary>
    /// Occurs when a saved message is deleted.
    /// </summary>
    public event SavedMessageDeletedEvent SavedMessageDeleted;

    /// <summary>
    /// Delegate for RECENT_MENTION_DELETE events when a recent mention is deleted.
    /// </summary>
    /// <param name="data">The recent mention data.</param>
    public delegate void RecentMentionDeletedEvent(RecentMentionDeleteGatewayData data);

    /// <summary>
    /// Occurs when a recent mention is deleted.
    /// </summary>
    public event RecentMentionDeletedEvent RecentMentionDeleted;

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
    public event MessageDeleteBulkEvent MessagesDeleted;

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
    public delegate void ChannelPinsUpdatedEvent(ChannelPinsUpdateGatewayData data);

    /// <summary>
    /// Occurs when a message is pinned or unpinned in a channel.
    /// </summary>
    public event ChannelPinsUpdatedEvent ChannelPinsUpdated;

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
    /// Delegate for CHANNEL_RECIPIENT_ADD events when a recipient is added to a channel.
    /// </summary>
    public delegate void GroupUserAddedEvent(Channel channel, SocketUser user);

    /// <summary>
    /// Occurs when a recipient is added to a group DM or channel.
    /// </summary>
    public event GroupUserAddedEvent GroupUserAdded;

    /// <summary>
    /// Delegate for CHANNEL_RECIPIENT_REMOVE events when a recipient is removed from a channel.
    /// </summary>
    public delegate void GroupUserRemovedEvent(Channel channel, SocketUser user);

    /// <summary>
    /// Occurs when a recipient is removed from a group DM or channel.
    /// </summary>
    public event GroupUserRemovedEvent GroupUserRemoved;

    // ============================================================================
    // Voice Events
    // ============================================================================

    /// <summary>
    /// Delegate for VOICE_STATE_UPDATE events when a user's voice state changes.
    /// </summary>
    /// <param name="data">The voice state data.</param>
    public delegate void VoiceStateUpdatedEvent(SocketVoiceState data);

    /// <summary>
    /// Occurs when a user joins, leaves, or updates their state in a voice channel.
    /// </summary>
    public event VoiceStateUpdatedEvent VoiceStateUpdated;

    /// <summary>
    /// Delegate for VOICE_SERVER_UPDATE events for voice connection data.
    /// </summary>
    /// <param name="data">The voice server data.</param>
    public delegate void VoiceServerUpdatedEvent(VoiceServer data);

    /// <summary>
    /// Occurs when voice server information is updated (used for establishing voice connections).
    /// </summary>
    public event VoiceServerUpdatedEvent VoiceServerUpdated;

    // ============================================================================
    // Guild Ban Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_BAN_ADD events when a user is banned from a guild.
    /// </summary>
    public delegate void GuildBanAddedEvent(SocketGuild guild, Cacheable<SocketGuildMember> member);

    /// <summary>
    /// Occurs when a user is banned from a guild.
    /// </summary>
    public event GuildBanAddedEvent UserBanned;

    /// <summary>
    /// Delegate for GUILD_BAN_REMOVE events when a user is unbanned from a guild.
    /// </summary>
    public delegate void GuildBanRemovedEvent(SocketGuild guild, ulong userId);

    /// <summary>
    /// Occurs when a user is unbanned from a guild.
    /// </summary>
    public event GuildBanRemovedEvent UserUnbanned;

    // ============================================================================
    // Webhook Events
    // ============================================================================

    /// <summary>
    /// Delegate for WEBHOOKS_UPDATE events when webhooks in a channel are updated.
    /// </summary>
    /// <param name="data">The webhooks update data.</param>
    public delegate void WebhooksUpdatedEvent(SocketGuild guild, Channel channel);

    /// <summary>
    /// Occurs when webhooks are created, updated, or deleted in a channel.
    /// </summary>
    public event WebhooksUpdatedEvent WebhooksUpdated;

    // ============================================================================
    // Guild Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_CREATE events when a guild becomes available.
    /// </summary>
    /// <param name="data">The guild data.</param>
    public delegate void GuildJoinedEvent(SocketGuild data);

    /// <summary>
    /// Occurs when the bot joins a guild.
    /// </summary>
    public event GuildJoinedEvent GuildJoined;

    /// <summary>
    /// Delegate for GUILD_UPDATE events when guild properties are updated.
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    public delegate void GuildUpdatedEvent(SocketGuild before, SocketGuild after);

    /// <summary>
    /// Occurs when a guild is updated (name, icon, settings, etc.).
    /// </summary>
    public event GuildUpdatedEvent GuildUpdated;

    /// <summary>
    /// Delegate for GUILD_DELETE events when the bot is removed.
    /// </summary>
    /// <param name="data">The guild delete data containing guild.</param>
    public delegate void GuildLeftEvent(SocketGuild data);

    /// <summary>
    /// Occurs when the bot is removed from a guild or when a guild becomes unavailable.
    /// Check the Unavailable property to distinguish between guild outages (true) and the user being removed (false/null).
    /// </summary>
    public event GuildLeftEvent GuildLeft;

    /// <summary>
    /// Delegate for GUILD_CREATE events when a guild has come back.
    /// </summary>
    /// <param name="data"></param>
    public delegate void GuildAvailableEvent(SocketGuild data);

    /// <summary>
    /// Occurs when the guild becomes available again.
    /// </summary>
    public event GuildAvailableEvent GuildAvailable;

    /// <summary>
    /// Delegate for GUILD_DELETE events when a guild has gone down.
    /// </summary>
    /// <param name="data"></param>
    public delegate void GuildUnavailableEvent(SocketGuild data);

    /// <summary>
    /// Occurs when the guild becomes unavailable.
    /// </summary>
    public event GuildUnavailableEvent GuildUnavailable;

    // ============================================================================
    // Guild Member Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_MEMBER_ADD events when a user joins a guild.
    /// </summary>
    /// <param name="data">The guild member data.</param>
    public delegate void GuildMemberAddedEvent(SocketGuildMember data);

    /// <summary>
    /// Occurs when a new member joins a guild.
    /// </summary>
    public event GuildMemberAddedEvent MemberJoined;

    /// <summary>
    /// Delegate for GUILD_MEMBER_UPDATE events when a guild member is updated.
    /// </summary>
    /// <param name="data">The updated guild member data.</param>
    public delegate void GuildMemberUpdatedEvent(SocketGuildMember data);

    /// <summary>
    /// Occurs when a guild member is updated (roles, nickname, avatar, etc.).
    /// </summary>
    public event GuildMemberUpdatedEvent MemberUpdated;

    /// <summary>
    /// Delegate for GUILD_MEMBER_REMOVE events when a member leaves or is removed from a guild.
    /// </summary>
    /// <param name="data">The entity data containing guild and user IDs.</param>
    public delegate void GuildMemberRemovedEvent(SocketGuild guild, Cacheable<SocketGuildMember> member);

    /// <summary>
    /// Occurs when a member leaves a guild or is kicked/banned.
    /// </summary>
    public event GuildMemberRemovedEvent MemberLeft;

    // ============================================================================
    // Guild Role Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_ROLE_CREATE events when a role is created in a guild.
    /// </summary>
    /// <param name="data">The guild role data containing guild ID and role information.</param>
    public delegate void GuildRoleCreatedEvent(SocketRole data);

    /// <summary>
    /// Occurs when a new role is created in a guild.
    /// </summary>
    public event GuildRoleCreatedEvent RoleCreated;

    /// <summary>
    /// Delegate for GUILD_ROLE_UPDATE events when a role is updated.
    /// </summary>
    /// <param name="data">The updated guild role data containing guild ID and role information.</param>
    public delegate void GuildRoleUpdatedEvent(SocketRole before, SocketRole after);

    /// <summary>
    /// Occurs when a role is updated (name, color, permissions, etc.).
    /// </summary>
    public event GuildRoleUpdatedEvent RoleUpdated;

    /// <summary>
    /// Delegate for GUILD_ROLE_DELETE events when a role is deleted from a guild.
    /// </summary>
    /// <param name="data">The guild role delete data containing guild and role IDs.</param>
    public delegate void GuildRoleDeletedEvent(SocketRole data);

    /// <summary>
    /// Occurs when a role is deleted from a guild.
    /// </summary>
    public event GuildRoleDeletedEvent RoleDeleted;

    // ============================================================================
    // Guild Emoji Events
    // ============================================================================

    public delegate void GuildEmojiCreatedEvent(Emoji data);

    public event GuildEmojiCreatedEvent EmojiCreated;

    public delegate void GuildEmojiDeletedEvent(Emoji data);

    public event GuildEmojiDeletedEvent EmojiDeleted;

    public delegate void GuildStickerCreatedEvent(Sticker data);

    public event GuildStickerCreatedEvent StickerCreated;

    public delegate void GuildStickerDeletedEvent(Sticker data);

    public event GuildStickerDeletedEvent StickerDeleted;

    /// <summary>
    /// Delegate for GUILD_EMOJIS_UPDATE events when guild emojis are updated.
    /// </summary>
    public delegate void GuildEmojiUpdatedEvent(Emoji before, Emoji after);

    /// <summary>
    /// Occurs when the list of emojis in a guild is updated (added, removed, or modified).
    /// </summary>
    public event GuildEmojiUpdatedEvent EmojiUpdated;

    /// <summary>
    /// Delegate for GUILD_STICKERS_UPDATE events when guild stickers are updated.
    /// </summary>
    public delegate void GuildStickerUpdatedEvent(Sticker before, Sticker after);

    /// <summary>
    /// Occurs when the list of stickers in a guild is updated (added, removed, or modified).
    /// </summary>
    public event GuildStickerUpdatedEvent StickerUpdated;

    // ============================================================================
    // Relationship Events
    // ============================================================================

    /// <summary>
    /// Delegate for RELATIONSHIP_ADD events when a relationship is added.
    /// </summary>
    /// <param name="data">The relationship data.</param>
    public delegate void RelationshipAddedEvent(RelationshipGatewayData data);

    /// <summary>
    /// Occurs when a relationship (friend, blocked user, etc.) is added.
    /// </summary>
    public event RelationshipAddedEvent RelationshipAdded;

    /// <summary>
    /// Delegate for RELATIONSHIP_UPDATE events when a relationship is updated.
    /// </summary>
    /// <param name="data">The relationship data.</param>
    public delegate void RelationshipUpdatedEvent(RelationshipGatewayData data);

    /// <summary>
    /// Occurs when a relationship is updated.
    /// </summary>
    public event RelationshipUpdatedEvent RelationshipUpdated;

    /// <summary>
    /// Delegate for RELATIONSHIP_REMOVE events when a relationship is removed.
    /// </summary>
    /// <param name="data">The relationship data.</param>
    public delegate void RelationshipRemovedEvent(RelationshipRemoveGatewayData data);

    /// <summary>
    /// Occurs when a relationship is removed.
    /// </summary>
    public event RelationshipRemovedEvent RelationshipRemoved;

    // ============================================================================
    // Favorite Meme Events
    // ============================================================================

    /// <summary>
    /// Delegate for FAVORITE_MEME_CREATE events when a favorite meme is created.
    /// </summary>
    /// <param name="data">The favorite meme data.</param>
    public delegate void FavoriteMediaCreatedEvent(FavoriteMediaGatewayData data);

    /// <summary>
    /// Occurs when a favorite meme is created.
    /// </summary>
    public event FavoriteMediaCreatedEvent FavoriteMediaCreated;

    /// <summary>
    /// Delegate for FAVORITE_MEME_UPDATE events when a favorite meme is updated.
    /// </summary>
    /// <param name="data">The favorite meme data.</param>
    public delegate void FavoriteMediaUpdatedEvent(FavoriteMediaGatewayData data);

    /// <summary>
    /// Occurs when a favorite meme is updated.
    /// </summary>
    public event FavoriteMediaUpdatedEvent FavoriteMediaUpdated;

    /// <summary>
    /// Delegate for FAVORITE_MEME_DELETE events when a favorite meme is deleted.
    /// </summary>
    /// <param name="data">The favorite meme data.</param>
    public delegate void FavoriteMediaDeletedEvent(FavoriteMediaDeleteGatewayData data);

    /// <summary>
    /// Occurs when a favorite meme is deleted.
    /// </summary>
    public event FavoriteMediaDeletedEvent FavoriteMediaDeleted;

    // ============================================================================
    // Call Events
    // ============================================================================

    /// <summary>
    /// Delegate for CALL_CREATE events when a call is created.
    /// </summary>
    /// <param name="data">The call data.</param>
    public delegate void CallCreatedEvent(CallCreateGatewayData data);

    /// <summary>
    /// Occurs when a call is created.
    /// </summary>
    public event CallCreatedEvent CallCreated;

    /// <summary>
    /// Delegate for CALL_UPDATE events when a call is updated.
    /// </summary>
    /// <param name="data">The call data.</param>
    public delegate void CallUpdatedEvent(CallUpdateGatewayData data);

    /// <summary>
    /// Occurs when a call is updated.
    /// </summary>
    public event CallUpdatedEvent CallUpdated;

    /// <summary>
    /// Delegate for CALL_DELETE events when a call is deleted.
    /// </summary>
    /// <param name="data">The call data.</param>
    public delegate void CallDeletedEvent(CallDeleteGatewayData data);

    /// <summary>
    /// Occurs when a call is deleted.
    /// </summary>
    public event CallDeletedEvent CallDeleted;

    // ============================================================================
    // Invite Events
    // ============================================================================

    /// <summary>
    /// Delegate for INVITE_CREATE events when an invite is created.
    /// </summary>
    /// <param name="data">The invite data including code, channel, guild, and inviter information.</param>
    public delegate void InviteCreatedEvent(Invite data);

    /// <summary>
    /// Occurs when a new invite is created for a guild or group DM.
    /// </summary>
    public event InviteCreatedEvent InviteCreated;

    /// <summary>
    /// Delegate for INVITE_DELETE events when an invite is deleted or expires.
    /// </summary>
    /// <param name="data">The invite data containing the deleted invite code and channel/guild information.</param>
    public delegate void InviteDeletedEvent(InviteDeleteGatewayData data);

    /// <summary>
    /// Occurs when an invite is deleted or expires.
    /// </summary>
    public event InviteDeletedEvent InviteDeleted;

    public delegate void GuildMemberCountsEvent(CountGatewayData<GuildMemberCountGatewayData> data);

    public event GuildMemberCountsEvent GuildMemberCounts;

    public delegate void ChannelMemberCountsEvent(CountGatewayData<GuildChannelMemberCountGatewayData> data);

    public event ChannelMemberCountsEvent ChannelMemberCounts;

    #endregion

    #endregion

    #region Voice

    /// <summary>
    /// Updates the current user's voice state (join/leave voice channels, mute, deafen).
    /// </summary>
    /// <param name="guildId">The guild ID containing the voice channel.</param>
    /// <param name="channelId">The voice channel ID to join, or null to disconnect.</param>
    /// <param name="selfMute">Whether the user should be self-muted.</param>
    /// <param name="selfDeaf">Whether the user should be self-deafened.</param>
    /// <remarks>
    /// This sends a VOICE_STATE_UPDATE packet to the gateway. The server will respond with
    /// VOICE_STATE_UPDATE and VOICE_SERVER_UPDATE events containing connection information.
    /// </remarks>
    public void UpdateVoiceState(ulong? guildId, ulong? channelId, bool selfMute, bool selfDeaf)
    {
        GatewayPacket packet = new GatewayPacket()
        {
            // TODO: Technically should be ulong, but fluxer expects strings. Fluxer will eventually be lenient and accept both.
            Data = JToken.FromObject(new VoiceStateUpdatePayload(guildId.ToString(), channelId.ToString(), selfMute, selfDeaf)),
            OpCode = FluxerOpCode.VoiceStateUpdate
        };
        SendGatewayPacket(packet);
    }

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

    ~FluxerGatewayClient()
    {
        Dispose(false);
    }

    #endregion
}
