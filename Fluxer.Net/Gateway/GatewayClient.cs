#undef NOPE
using Fluxer.Net.RateLimiting;
using Fluxer.Net.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
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
/// <item>Event filtering via <see cref="FluxerGatewayConfig.IgnoredEvents"/></item>
/// </list>
/// <para>
/// This client should be paired with <see cref="FluxerApiClient"/> for full Fluxer functionality.
/// </para>
/// </remarks>
public partial class FluxerGatewayClient : IDisposable
{
    #region Declares
    private readonly FluxerClient _client;
    private WebsocketClient _webSocket;
    private readonly Stopwatch _gatewayDuration = new();
    private long _heartbeatSentAt;

    /// <summary>
    /// Milliseconds between the last heartbeat and its acknowledgement, or 0 before the first one.
    /// </summary>
    public int Latency { get; private set; }

    /// <summary>
    /// Whether the gateway connection is currently open.
    /// </summary>
    public bool IsConnected => _webSocket?.IsRunning == true;

    public FluxerGatewayConfig Config { get; internal set; }

    /// <summary>
    /// Zero based id of the shard the server reported in READY, or null when the session is not sharded.
    /// </summary>
    public int? ShardId { get; private set; }

    /// <summary>
    /// Total number of shards the server reported in READY, or null when the session is not sharded.
    /// </summary>
    public int? ShardCount { get; private set; }

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

    private readonly ILogger _logger;
    private readonly GatewaySendQueue _sendQueue;
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
        _sendQueue = new GatewaySendQueue(WriteToSocket, _logger);
        _logger.Information("Initialized Fluxer.Net gateway client ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _client.Config.Version);
    }
    #endregion

    #region Cache

    public SocketCurrentUser? CurrentUser { get; internal set; }

    internal ulong? OwnerId { get; set; }

    public async Task<GroupChannel?> GetGroupChannelAsync(ulong channelId)
    {
        if (Channels.TryGetValue(channelId, out Channel? channel))
            return channel as GroupChannel;

        channel = await _client.Rest.GetChannelAsync(channelId);
        if (channel == null || channel is not GroupChannel gc)
            return null;

        Channels.TryAdd(channel.Id, gc);
        return gc;
    }

    public async Task<DMChannel?> GetDMChannelAsync(ulong channelId)
    {
        if (Channels.TryGetValue(channelId, out Channel? channel))
            return channel as DMChannel;

        channel = await _client.Rest.GetChannelAsync(channelId);
        if (channel == null || channel is not DMChannel dm)
            return null;

        Channels.TryAdd(channel.Id, dm);
        return dm;
    }

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

    /// <summary>
    /// The stores the gateway keeps earlier states in, set up from <see cref="FluxerGatewayConfig.Cache"/> on connect.
    /// </summary>
    public GatewayCache Cache { get; internal set; } = new GatewayCache(new FluxerCacheConfig());

    private bool _cacheConfigured;

    /// <summary>
    /// Gets a guild message from <see cref="GatewayCache.Messages"/>, or null when it isn't cached or its guild or channel is gone.
    /// </summary>
    public async ValueTask<SocketMessage?> GetCachedMessageAsync(ulong messageId)
        => ToSocketMessage(await Cache.Messages.GetAsync(messageId));

    private SocketMessage? ToSocketMessage(MessageGatewayData? data)
    {
        if (data?.GuildId is not ulong guildId || !Guilds.TryGetValue(guildId, out SocketGuild guild)
            || !Channels.TryGetValue(data.ChannelId, out Channel channel))
            return null;

        return SocketMessage.Create(_client, data, channel, guild);
    }

    private static CacheScope ScopeOf(MessageGatewayData data) => new CacheScope(data.GuildId, data.ChannelId);

    private static MessageGatewayData MergeMessage(MessageGatewayData cached, JObject update)
    {
        JObject merged = JObject.FromObject(cached, FluxerClient._gatewaySerializer);
        merged.Merge(update, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
        return merged.ToObject<MessageGatewayData>(FluxerClient._gatewaySerializer) ?? cached;
    }

    private async ValueTask TrackUserAsync(UserJson? user)
    {
        if (user == null || string.IsNullOrEmpty(user.Username))
            return;

        UserJson? before = await Cache.Users.GetAsync(user.Id);
        await Cache.Users.SetAsync(user.Id, before == null ? user : FillMissingUserFields(user, before), default);
        if (before != null && (before.Username != user.Username || before.Discriminator != user.Discriminator
            || before.AvatarHash != user.AvatarHash
            || (user.DisplayName != null && before.DisplayName != user.DisplayName)
            || (user.Flags != 0 && before.Flags != 0 && before.Flags != user.Flags)))
            UserChanged?.Invoke(User.Create(_client, before), User.Create(_client, user));
    }

    private static UserJson FillMissingUserFields(UserJson user, UserJson before)
    {
        bool missingName = user.DisplayName == null && before.DisplayName != null;
        bool missingFlags = user.Flags == 0 && before.Flags != 0;
        if (!missingName && !missingFlags)
            return user;

        UserJson filled = JObject.FromObject(user, FluxerClient._gatewaySerializer).ToObject<UserJson>(FluxerClient._gatewaySerializer) ?? user;
        if (missingName)
            filled.DisplayName = before.DisplayName;
        if (missingFlags)
            filled.Flags = before.Flags;
        return filled;
    }

    private async ValueTask TrackPresenceAsync(PresenceGatewayData presence)
    {
        PresenceGatewayData? before = presence.User == null ? null : await Cache.Presences.GetAsync(presence.User.Id);
        if (presence.User != null)
            await Cache.Presences.SetAsync(presence.User.Id, presence, default);

        PresenceUpdated?.Invoke(presence);
        PresenceChanged?.Invoke(before, presence);
        await TrackUserAsync(presence.User);
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
    public async Task ConnectAsync(FluxerGatewayConfig? config = null)
    {
        if (config == null)
            config = new FluxerGatewayConfig();
        Config = config;
        if (!_cacheConfigured)
        {
            Cache = new GatewayCache(config.Cache, _logger);
            _cacheConfigured = true;
        }

        StartEventDispatch(config.Events);
        SeedPresence(config.Presence);

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

            _sendQueue.ResetConnection();
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

            GatewayPacket login = CreateIdentifyPacket(new Dictionary<string, string>
            {
                { "os", "linux" },
                { "browser", "fluxer-net" },
                { "device", "fluxer-net" }
            });

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
        {
            TaskCompletionSource<bool> existing = guild._downloaderPromise;
            if (await Task.WhenAny(existing.Task, Task.Delay(new TimeSpan(0, 0, 30))) != existing.Task)
                return false;

            return existing.Task.Result;
        }

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

        TaskCompletionSource<bool> promise = guild._downloaderPromise;
        if (await Task.WhenAny(promise.Task, Task.Delay(new TimeSpan(0, 0, 30))) != promise.Task)
        {
            promise.TrySetResult(false);
            guild._downloaderPromise = null;
            return false;
        }

        return promise.Task.Result;
    }

    /// <summary>
    /// Sends a gateway packet to the Fluxer WebSocket server.
    /// </summary>
    /// <typeparam name="T">The type of the gateway packet data.</typeparam>
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
        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Sending serialized gateway packet {Enums}", LogRedaction.Redact(text));

        _sendQueue.Enqueue(text, (Data as GatewayPacket)?.OpCode);
    }

    private void WriteToSocket(string text)
    {
        try
        {
            _webSocket?.Send(text);
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

    internal void GatewayMessageHandler(string message)
    {
        // try deserializing the packet, fallback to regex
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received raw gateway message {Message}", LogRedaction.Redact(message));

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
                    TrackReceivedBacklog(Interlocked.Increment(ref _pendingDispatches));
                    _ = DispatchInOrderAsync(packet);
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
                    {
                        GatewayErrorGatewayData? data = packet.Data?.ToObject<GatewayErrorGatewayData>(FluxerClient._gatewaySerializer);
                        if (data != null)
                        {
                            _logger.Debug("Received GatewayError {Code}: {Message}", data.Code, data.Message);
                            GatewayErrorReceived?.Invoke(data);
                        }
                        else
                            _logger.Warning("GatewayError opcode received but data could not be cast to GatewayErrorGatewayData");
                    }
                    return;
                case FluxerOpCode.VoiceSignal:
                    _logger.Debug("Received VoiceSignal opcode from server (unexpected)");
                    return;
                case FluxerOpCode.LazyRequest:
                    _logger.Debug("Received LazyRequest opcode from server (unexpected)");
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

    private static MessageReactionGatewayData ToReactionData(MessageReactionAddManyGatewayData batch, MessageReactionAddManyEntryGatewayData entry)
        => new MessageReactionGatewayData
        {
            UserId = entry.UserId,
            ChannelId = batch.ChannelId,
            MessageId = batch.MessageId,
            GuildId = batch.GuildId,
            Emoji = entry.Emoji,
            Member = entry.Member
        };

    private const int IdentifyFlagDebounceMessageReactions = 1 << 1;

    internal GatewayPacket CreateIdentifyPacket(Dictionary<string, string> properties)
        => new GatewayPacket
        {
            OpCode = FluxerOpCode.Identify,
            Data = JToken.FromObject(new IdentifyGatewayData(_client.RawToken)
            {
                Properties = properties,
                IgnoredGatewayEvents = Config.IgnoredEvents,
                Presence = Config.Presence,
                Flags = Config.DebounceMessageReactions ? IdentifyFlagDebounceMessageReactions : null,
                Shard = _client.Config.GetShard()
            })
        };

    private readonly SemaphoreSlim _dispatchLock = new(1, 1);
    private int _pendingDispatches;

    /// <summary>
    /// The number of received events not yet applied to the cache, usually behind a slow cache store or busy handlers.
    /// </summary>
    public int PendingEvents => Volatile.Read(ref _pendingDispatches);

    /// <summary>
    /// Waits until every event received so far has been handled and its handlers have finished, for example before
    /// shutting down.
    /// </summary>
    public async Task WaitForPendingEventsAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            await _dispatchLock.WaitAsync(cancellationToken);
            _dispatchLock.Release();
            await WaitForQueuedEventsAsync(0, cancellationToken);
            if (Volatile.Read(ref _pendingDispatches) == 0 && Volatile.Read(ref _queuedEvents) == 0 || _dispatchStopped)
                return;

            await Task.Delay(1, cancellationToken);
        }
    }

    private async Task DispatchInOrderAsync(GatewayPacket p)
    {
        await _dispatchLock.WaitAsync();
        try
        {
            _dispatchScope.Value = new DispatchScope(PartitionOf(p));
            await HandleDispatchAsync(p);
            await WaitForHandlerRoomAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error handling {Dispatch} event", p.Dispatch);
        }
        finally
        {
            _dispatchScope.Value = null;
            TrackReceivedBacklog(Interlocked.Decrement(ref _pendingDispatches));
            _dispatchLock.Release();
        }
    }

    private async ValueTask ForgetUsersAsync(IEnumerable<ulong> userIds, ulong leftGuildId)
    {
        HashSet<ulong> gone = new HashSet<ulong>(userIds);
        foreach (SocketGuild other in Guilds.Values)
        {
            if (other.Id == leftGuildId)
                continue;

            foreach (ulong memberId in other.Members.Keys)
                gone.Remove(memberId);
            if (gone.Count == 0)
                return;
        }

        if (gone.Count == 0)
            return;

        await Cache.Presences.RemoveManyAsync(gone);
        await Cache.Users.RemoveManyAsync(gone);
    }

    private async Task HandleDispatchAsync(GatewayPacket p)
    {
        //Console.WriteLine("Type: " + p.Dispatch);
        //Console.WriteLine(p.Data);
        switch (p.Dispatch)
        {
            case "READY":
                {
                    ReadyGatewayData? data = p.Data.ToObject<ReadyGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        await Cache.ClearBuiltInAsync();
                        CurrentUser = SocketCurrentUser.Create(_client, data.User);
                        CurrentUser.SessionId = data.SessionId;
                        GatewaySessionJson? presence = data.Sessions.FirstOrDefault(x => x.SessionId == CurrentUser.SessionId);
                        _sessionId = data.SessionId;
                        ShardId = data.Shard?[0];
                        ShardCount = data.Shard?[1];
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
                                if (g.Members == null)
                                    continue;

                                CurrentMembers.TryAdd(g.Id, SocketGuildMember.Create(_client, g.Members.First(x => x.Id == CurrentUser.Id)));
                                SocketGuild guild = SocketGuild.Create(_client, g, CurrentMembers[g.Id]);
                                foreach (GuildMemberGatewayData m in g.Members)
                                {
                                    if (m.Id != CurrentUser.Id)
                                    {
                                        SocketGuildMember member = SocketGuildMember.Create(_client, m);
                                        member.GuildId = guild.Id;
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
                                    channel.GuildId = g.Id;
                                    if (!Channels.TryAdd(c.Id, channel))
                                    {
                                        channel = Channels[c.Id];
                                        channel.Update(c);
                                    }
                                    guild.Channels.TryAdd(c.Id, channel);
                                }

                                // Add threads
                                foreach (ChannelGatewayData c in g.Threads)
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
                                {
                                    using (EventsFor(guild.Id))
                                        GuildAvailable?.Invoke(guild);
                                }
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
                        ConnectionsUpdated?.Invoke(data.Connections.Select(x => UserConnection.Create(_client, x)).ToArray());
                    else
                        _logger.Warning("USER_CONNECTIONS_UPDATE event received but data could not be cast to ConnectionsUpdatedGatewayData");
                }
                break;
            case "WEBAUTHN_CREDENTIALS_UPDATE":
                {
                    WebAuthnCredentialJson[]? data = p.Data.ToObject<WebAuthnCredentialJson[]>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        WebAuthnCredentialsUpdated?.Invoke(data.Select(x => WebAuthnCredential.Create(_client, x)).ToArray());
                    else
                        _logger.Warning("WEBAUTHN_CREDENTIALS_UPDATE event received but data could not be cast to WebAuthnCredentialJson");
                }
                break;

            // Relationships
            case "RELATIONSHIP_ADD":
                {
                    RelationshipGatewayData? data = p.Data.ToObject<RelationshipGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RelationshipAdded?.Invoke(SocketRelationship.Create(_client, data, SocketUser.Create(_client, data.User)));
                    else
                        _logger.Warning("RELATIONSHIP_ADD event received but data could not be cast to RelationshipGatewayData");
                }
                return;
            case "RELATIONSHIP_UPDATE":
                {
                    RelationshipGatewayData? data = p.Data.ToObject<RelationshipGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RelationshipUpdated?.Invoke(SocketRelationship.Create(_client, data, SocketUser.Create(_client, data.User)));
                    else
                        _logger.Warning("RELATIONSHIP_UPDATE event received but data could not be cast to RelationshipGatewayData");
                }
                return;
            case "RELATIONSHIP_REMOVE":
                {
                    RelationshipRemoveGatewayData? data = p.Data.ToObject<RelationshipRemoveGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        RelationshipRemoved?.Invoke(data.Id);
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
                        RecentMentionDeleted?.Invoke(data.MessageId);
                    else
                        _logger.Warning("RECENT_MENTION_DELETE event received but data could not be cast to RecentMentionDeleteGatewayData");
                }
                return;

            // Favorite media
            case "FAVORITE_MEME_CREATE":
                {
                    FavoriteMediaGatewayData? data = p.Data.ToObject<FavoriteMediaGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        FavoriteMediaCreated?.Invoke(FavoriteMedia.Create(_client, data));
                    else
                        _logger.Warning("FAVORITE_MEME_CREATE event received but data could not be cast to FavoriteMemeGatewayData");
                }
                return;
            case "FAVORITE_MEME_UPDATE":
                {
                    FavoriteMediaGatewayData? data = p.Data.ToObject<FavoriteMediaGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        FavoriteMediaUpdated?.Invoke(FavoriteMedia.Create(_client, data));
                    else
                        _logger.Warning("FAVORITE_MEME_UPDATE event received but data could not be cast to FavoriteMemeGatewayData");
                }
                return;
            case "FAVORITE_MEME_DELETE":
                {
                    FavoriteMediaDeleteGatewayData? data = p.Data.ToObject<FavoriteMediaDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        FavoriteMediaDeleted?.Invoke(data.Id);
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
                            guild.MemberCount = data.MemberCount;
                            guild.OnlineCount = data.OnlineCount;
                        }

                        guild.IsAvailable = true;
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

                        // Add or update threads
                        foreach (ChannelGatewayData c in data.Threads)
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
                    else
                        _logger.Warning("GUILD_CREATE event received but data could not be cast to GuildGatewayData");
                }
                return;
            case "GUILD_SYNC":
                {
                    GuildGatewayData? data = p.Data.ToObject<GuildGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
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
                            guild.MemberCount = data.MemberCount;
                            guild.OnlineCount = data.OnlineCount;
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
                            Channel channel = Channel.Create(_client, c);
                            channel.GuildId = data.Id;
                            if (!Channels.TryAdd(c.Id, channel))
                            {
                                channel = Channels[c.Id];
                                Channel before = channel.Clone();
                                if (before.Type != c.Type)
                                {
                                    channel = Channel.Create(_client, c);
                                    Channels.TryUpdate(c.Id, channel, before);
                                    if (c.GuildId.HasValue && Guilds.TryGetValue(c.GuildId.Value, out SocketGuild getGuild))
                                        getGuild.Channels.TryUpdate(c.Id, channel, before);
                                }
                                else
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
                    }
                    else
                        _logger.Warning("GUILD_SYNC event received but data could not be cast to GuildGatewayData");
                }
                break;
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
                        if (data.IsUnavailable.HasValue && data.IsUnavailable.Value)
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
                            await Cache.RemoveScopeAsync(data.Id);
                            if (guild != null)
                                await ForgetUsersAsync(guild.Members.Keys.ToList(), data.Id);
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
                            guild.Roles[role.Id] = role;
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
                    if (Config.DisableEmojiCache)
                        return;

                    GuildEmojisUpdateGatewayData? data = p.Data.ToObject<GuildEmojisUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            return;

                        IEnumerable<GuildEmojiJson> newEmojis = data.Emojis.Where(x => !guild.Emojis.ContainsKey(x.Id.Value));
                        IEnumerable<KeyValuePair<ulong, GuildEmoji>> deletedEmojis = guild.Emojis.Where(x => !data.Emojis.Any(y => y.Id == x.Key));
                        (GuildEmoji Entity, GuildEmojiJson Model)[] updatedEmojis = data.Emojis.Select(x =>
                        {
                            GuildEmoji s = guild.Emojis.GetValueOrDefault(x.Id.Value);
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
                    if (Config.DisableStickerCache)
                        return;

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
                        SocketGuild? createGuild = data.GuildId.HasValue ? GetGuild(data.GuildId.Value) : null;
                        Channel channel = SocketChannel.Create(_client, data, createGuild);
                        if (!Channels.TryAdd(channel.Id, channel))
                        {
                            channel = Channels[channel.Id];
                            channel.Update(data);
                        }

                        if (data.GuildId.HasValue)
                        {
                            channel.GuildId = data.GuildId;

                            SocketGuild? guild = GetGuild(data.GuildId.Value);

                            if (guild == null)
                                return;

                            guild.Channels.TryAdd(channel.Id, channel);
                            ChannelCreated?.Invoke(guild, channel);
                        }
                        else
                        {
                            if (channel is DMChannel dm)
                                DMCreated?.Invoke(dm);
                            else if (channel is GroupChannel gc)
                                GroupJoined?.Invoke(gc);
                        }
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
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (Channels.TryGetValue(data.Id, out Channel channel))
                            {
                                Channel before = channel.Clone();
                                if (before.Type != data.Type)
                                {
                                    channel = SocketChannel.Create(_client, data, guild);
                                    Channels[data.Id] = channel;
                                    guild.Channels[data.Id] = channel;
                                }
                                else
                                    channel.Update(data);
                                ChannelUpdated?.Invoke(guild, before, channel);
                            }
                            else
                            {
                                channel = SocketChannel.Create(_client, data, guild);
                                Channels.TryAdd(data.Id, channel);
                                guild.Channels.TryAdd(data.Id, channel);
                            }
                        }
                        else
                        {
                            if (!Channels.TryGetValue(data.Id, out Channel channel))
                            {
                                channel = SocketChannel.Create(_client, data, null);
                                Channels.TryAdd(data.Id, channel);
                            }
                            else
                                channel.Update(data);

                            if (channel is GroupChannel gc)
                                GroupUpdated?.Invoke(gc);
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
                        if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            return;

                        foreach (ChannelJson c in data.Channels)
                        {
                            if (Channels.TryGetValue(c.Id, out Channel channel))
                            {
                                Channel before = channel.Clone();
                                if (before.Type != c.Type)
                                {
                                    channel = SocketChannel.Create(_client, c, guild);
                                    Channels[c.Id] = channel;
                                    guild.Channels[c.Id] = channel;
                                }
                                else
                                    channel.Update(c);
                                ChannelUpdated?.Invoke(guild, before, channel);
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
                        if (data.Type == ChannelType.Group)
                        {
                            GroupLeft?.Invoke((GroupChannel)Channel.Create(_client, data));
                        }
                        else if (data.Type == ChannelType.DM)
                        {

                        }
                        else
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;
                            guild.Channels.TryRemove(data.Id, out _);

                            await Cache.RemoveScopeAsync(data.Id);
                            ChannelDeleted?.Invoke(guild, channel ?? SocketChannel.Create(_client, data, guild));
                        }
                    }
                    else
                        _logger.Warning("CHANNEL_DELETE event received but data could not be cast to ChannelGatewayData");
                }
                return;

            case "THREAD_CREATE":
                {
                    ChannelGatewayData? data = p.Data.ToObject<ChannelGatewayData>(FluxerClient._gatewaySerializer);
                    if (!data.GuildId.HasValue || !Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                        return;

                    Channel channel = SocketChannel.Create(_client, data, guild);
                    channel.GuildId = data.GuildId;
                    if (!Channels.TryAdd(data.Id, channel))
                    {
                        channel = Channels[data.Id];
                        channel.Update(data);
                    }
                    guild.Channels.TryAdd(data.Id, channel);

                    if (data.IsThreadNew.GetValueOrDefault())
                        ThreadCreated?.Invoke(guild, Channel.Create(_client, data) as ThreadChannel);
                    else
                        ThreadJoined?.Invoke(guild, Channel.Create(_client, data) as ThreadChannel);
                }
                break;
            case "THREAD_UPDATE":
                {
                    ChannelGatewayData? data = p.Data.ToObject<ChannelGatewayData>(FluxerClient._gatewaySerializer);
                    if (!data.GuildId.HasValue || !Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                        return;

                    Channel? threadBefore = null;
                    if (Channels.TryGetValue(data.Id, out Channel? cachedThread))
                    {
                        threadBefore = cachedThread.Clone();
                        cachedThread.Update(data);
                    }
                    else
                    {
                        cachedThread = SocketChannel.Create(_client, data, guild);
                        Channels.TryAdd(data.Id, cachedThread);
                        guild.Channels.TryAdd(data.Id, cachedThread);
                    }

                    ThreadChannel channel = (ThreadChannel)Channel.Create(_client, data);
                    ThreadUpdated?.Invoke(guild, new Cacheable<ThreadChannel>(data.Id, null, () =>
                    {
                        return null;
                    }), channel);
                    ThreadChanged?.Invoke(guild, threadBefore, cachedThread);
                }
                break;

            case "THREAD_DELETE":
                {
                    ChannelGatewayData? data = p.Data.ToObject<ChannelGatewayData>(FluxerClient._gatewaySerializer);
                    if (!data.GuildId.HasValue || !Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                        return;

                    Channels.TryRemove(data.Id, out Channel channel);
                    guild.Channels.TryRemove(data.Id, out _);
                    await Cache.RemoveScopeAsync(data.Id);

                    if (channel is IThreadChannel)
                    {
                        data.Name ??= channel.Name;
                        data.ParentId ??= channel.ParentId;
                    }

                    ThreadDeleted?.Invoke(guild, new Cacheable<ThreadChannel>(data.Id, channel is IThreadChannel ? Channel.Create(_client, data) as ThreadChannel : null, () =>
                    {
                        return null;
                    }));
                }
                break;
            case "THREAD_MEMBER_LIST_UPDATE":
                {
                    ThreadMemberListUpdateGatewayData? data = p.Data.ToObject<ThreadMemberListUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ThreadMemberListUpdated?.Invoke(data);
                    else
                        _logger.Warning("THREAD_MEMBER_LIST_UPDATE event received but data could not be cast to ThreadMemberListUpdateGatewayData");
                }
                return;
            case "FORUM_UNREADS":
                {
                    ForumUnreadsGatewayData? data = p.Data.ToObject<ForumUnreadsGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ForumUnreadsReceived?.Invoke(data);
                    else
                        _logger.Warning("FORUM_UNREADS event received but data could not be cast to ForumUnreadsGatewayData");
                }
                return;
            case "GUILD_MEMBER_LIST_UPDATE":
                {
                    GuildMemberListUpdateGatewayData? data = p.Data.ToObject<GuildMemberListUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        GuildMemberListUpdated?.Invoke(data);
                    else
                        _logger.Warning("GUILD_MEMBER_LIST_UPDATE event received but data could not be cast to GuildMemberListUpdateGatewayData");
                }
                return;
            case "THREAD_MEMBERS_UPDATE":
                {
                    ThreadMembersUpdateGatewayData? data = p.Data.ToObject<ThreadMembersUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (!Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        return;

                    if (data.AddedMembers != null && data.AddedMembers.Any())
                        ThreadMembersAdded?.Invoke(guild, new Cacheable<ThreadChannel>(data.ThreadId, null, async () =>
                        {
                            return await _client.Rest.GetThreadChannelAsync(data.ThreadId);
                        }), data.AddedMembers.Select(x => ThreadMember.Create(_client, x)).ToArray());

                    if (data.RemovedMemberIds != null && data.RemovedMemberIds.Any())
                    {
                        if (data.RemovedMemberIds.Any(x => x == CurrentUser.Id))
                            ThreadLeft?.Invoke(guild, new Cacheable<ThreadChannel>(data.ThreadId, null, async () =>
                            {
                                return await _client.Rest.GetThreadChannelAsync(data.ThreadId);
                            }));

                        ThreadMembersRemoved?.Invoke(guild, new Cacheable<ThreadChannel>(data.ThreadId, null, async () =>
                        {
                            return await _client.Rest.GetThreadChannelAsync(data.ThreadId);
                        }), data.RemovedMemberIds);
                    }

                    if (Channels.TryGetValue(data.ThreadId, out Channel? countedThread))
                        countedThread.MemberCount = data.MemberCount;

                    ThreadMembersCount?.Invoke(guild, new Cacheable<ThreadChannel>(data.ThreadId, null, async () =>
                    {
                        return await _client.Rest.GetThreadChannelAsync(data.ThreadId);
                    }), data.MemberCount);
                }
                break;

            case "THREAD_LIST_SYNC":
                {
                    ThreadListSyncGatewayData? data = p.Data.ToObject<ThreadListSyncGatewayData>(FluxerClient._gatewaySerializer);
                    if (data == null || !Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        return;

                    List<ThreadChannel> threads = new List<ThreadChannel>();
                    foreach (ChannelGatewayData thread in data.Threads ?? Array.Empty<ChannelGatewayData>())
                    {
                        if (Channels.TryGetValue(thread.Id, out Channel? cachedThread))
                            cachedThread.Update(thread);
                        else
                        {
                            Channel socketThread = SocketChannel.Create(_client, thread, guild);
                            socketThread.GuildId = data.GuildId;
                            Channels.TryAdd(thread.Id, socketThread);
                            guild.Channels.TryAdd(thread.Id, socketThread);
                        }

                        if (Channel.Create(_client, thread) is ThreadChannel restThread)
                            threads.Add(restThread);
                    }

                    ThreadMemberJson[] memberData = data.Members ?? Array.Empty<ThreadMemberJson>();
                    foreach (ThreadMemberJson member in memberData)
                    {
                        if (Channels.TryGetValue(member.ThreadId, out Channel? memberThread))
                            memberThread.SetThreadMember(member);
                    }

                    ThreadListSynced?.Invoke(guild, threads.ToArray(), memberData.Select(x => ThreadMember.Create(_client, x)).ToArray(), data.ChannelIds);
                }
                break;
            case "THREAD_MEMBER_UPDATE":
                {
                    ThreadMemberUpdateGatewayData? data = p.Data.ToObject<ThreadMemberUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data == null || !Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        return;

                    if (Channels.TryGetValue(data.ThreadId, out Channel? thread))
                        thread.SetThreadMember(data);

                    ThreadMemberUpdated?.Invoke(guild, ThreadMember.Create(_client, data));
                }
                break;

            // Groups
            case "CHANNEL_RECIPIENT_ADD":
                {
                    ChannelRecipientGatewayData? data = p.Data.ToObject<ChannelRecipientGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            GroupUserAdded?.Invoke(new Cacheable<GroupChannel>(data.ChannelId, null, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null || channel is not GroupChannel gc)
                                    return null;

                                Channels.TryAdd(channel.Id, gc);
                                return gc;
                            }), SocketUser.Create(_client, data.User));
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
                            GroupUserRemoved?.Invoke(new Cacheable<GroupChannel>(data.ChannelId, null, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null || channel is not GroupChannel gc)
                                    return null;

                                Channels.TryAdd(channel.Id, gc);
                                return gc;
                            }), SocketUser.Create(_client, data.User));
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
                    InviteCreateGatewayData? data = p.Data.ToObject<InviteCreateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        await Cache.Invites.SetAsync(data.Code, data, new CacheScope(data.GuildId ?? data.Guild?.Id, data.ChannelId ?? data.Channel?.Id));
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
                    {
                        InviteJson? removedInvite = await Cache.Invites.RemoveAsync(data.Code);
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                        {
                            SocketGuild? inviteGuild = data.GuildId.HasValue ? GetGuild(data.GuildId.Value) : null;
                            InviteDeleted?.Invoke(channel, inviteGuild, data.Code);
                            InviteRemoved?.Invoke(channel, inviteGuild, data.Code, removedInvite == null ? null : Invite.Create(_client, removedInvite));
                        }
                    }
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
                            if (guild.MemberCount.HasValue)
                                guild.MemberCount++;
                            MemberJoined?.Invoke(member);
                            await TrackUserAsync(data.User);
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
                            SocketGuildMember? before = guild.Members.TryGetValue(data.Id, out SocketGuildMember cached)
                                ? (SocketGuildMember)cached.Snapshot()
                                : null;
                            SocketGuildMember member = guild.AddOrUpdateMember(data);
                            MemberUpdated?.Invoke(member);
                            MemberChanged?.Invoke(before, member);
                            await TrackUserAsync(data.User);
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
                            if (guild.MemberCount > 0)
                                guild.MemberCount--;
                            MemberLeft?.Invoke(guild, new Cacheable<SocketGuildMember>(userId, member, () =>
                            {
                                return null;
                            }));
                            await ForgetUsersAsync(new[] { userId }, data.GuildId);
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
                                    guild._downloaderPromise.TrySetResult(true);
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
                    GuildAuditLogJson? data = p.Data.ToObject<GuildAuditLogJson>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                        {
                            SocketAuditLog auditLog = SocketAuditLog.Create(_client, data);
                            AuditLogCreated?.Invoke(guild, auditLog);
                        }
                    }
                    else
                        _logger.Warning("GUILD_AUDIT_LOG_ENTRY_CREATE event received but data could not be cast to GuildAuditLogEntryJson");
                }
                break;

            // Bans
            case "GUILD_BAN_ADD":
                {
                    GuildBanGatewayData? data = p.Data.ToObject<GuildBanGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
                            UserBanned?.Invoke(guild, new Cacheable<SocketGuildMember>(data.User.Id, guild.Members.GetValueOrDefault(data.User.Id), () =>
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
                        if (Guilds.TryGetValue(data.GuildId, out SocketGuild guild))
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
                        await TrackPresenceAsync(data);
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
                            await TrackPresenceAsync(i);
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
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            channel.LastMessageId = data.Id;
                            SocketMessage created = SocketMessage.Create(_client, data, channel, guild);
                            await Cache.Messages.SetAsync(data.Id, data, ScopeOf(data));
                            await TrackUserAsync(data.Author);
                            MessageCreated?.Invoke(created);
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageCreated?.Invoke(SocketPrivateMessage.Create(_client, data, new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            })));
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_CREATE event received but data could not be cast to MessageGatewayData");
                }
                return;
            case "MESSAGE_UPDATE":
                {
                    MessageGatewayData? data = p.Data.ToObject<MessageGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            MessageGatewayData? cached = await Cache.Messages.GetAsync(data.Id);
                            SocketMessage? before = ToSocketMessage(cached);
                            if (cached != null)
                            {
                                data = MergeMessage(cached, (JObject)p.Data);
                                await Cache.Messages.SetAsync(data.Id, data, ScopeOf(data));
                            }

                            SocketMessage edited = SocketMessage.Create(_client, data, channel, guild);
                            MessageUpdated?.Invoke(edited);
                            MessageEdited?.Invoke(before, edited);
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageUpdated?.Invoke(SocketPrivateMessage.Create(_client, data, new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            })));
                        }
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
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            SocketMessage? removed = ToSocketMessage(await Cache.Messages.RemoveAsync(data.MessageId));
                            MessageDeleted?.Invoke(channel, data.AuthorId, data.MessageId, data.Content);
                            MessageRemoved?.Invoke(channel, data.MessageId, removed);
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageDeleted?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), data.AuthorId, data.MessageId, data.Content);
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_DELETE event received but data could not be cast to EntityRemovedGatewayData");
                }
                return;
            case "MESSAGE_DELETE_BULK":
                {
                    MessageBulkDeleteGatewayData? data = p.Data.ToObject<MessageBulkDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            List<SocketMessage> removed = new List<SocketMessage>();
                            foreach (ulong id in data.Ids)
                            {
                                if (ToSocketMessage(await Cache.Messages.RemoveAsync(id)) is { } message)
                                    removed.Add(message);
                            }
                            MessagesDeleted?.Invoke(channel, data.GuildId.HasValue ? GetGuild(data.GuildId.Value) : null, data.Ids);
                            MessagesRemoved?.Invoke(channel, data.Ids.ToList(), removed);
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessagesDeleted?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), data.Ids);
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_DELETE_BULK event received but data could not be cast to MessageBulkDeleteGatewayData");
                }
                return;
            case "MESSAGE_ACK":
                {
                    MessageAckGatewayData? data = p.Data.ToObject<MessageAckGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        Channels.TryGetValue(data.ChannelId, out Channel channel);

                        MessageAck?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                        {
                            Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                            if (channel == null)
                                return null;

                            if (channel.Type == ChannelType.DM || channel.Type == ChannelType.Group)
                                Channels.TryAdd(data.ChannelId, channel);

                            return channel;
                        }), new Cacheable<Message>(data.MessageId, null, async () =>
                        {
                            return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                        }));
                    }
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
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            SocketGuildMember member = guild.AddOrUpdateMember(data.Member);

                            MessageReactionAdded?.Invoke(channel, new Cacheable<Message>(data.MessageId, await GetCachedMessageAsync(data.MessageId), async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }), SocketReaction.Create(_client, data, member));
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageReactionAdded?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), new Cacheable<Message>(data.MessageId, null, async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }), SocketReaction.Create(_client, data, null));
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_REACTION_ADD event received but data could not be cast to MessageReactionGatewayData");
                }
                return;
            case "MESSAGE_REACTION_ADD_MANY":
                {
                    MessageReactionAddManyGatewayData? data = p.Data.ToObject<MessageReactionAddManyGatewayData>(FluxerClient._gatewaySerializer);
                    if (data == null)
                    {
                        _logger.Warning("MESSAGE_REACTION_ADD_MANY event received but data could not be cast to MessageReactionAddManyGatewayData");
                        return;
                    }

                    if (data.GuildId.HasValue)
                    {
                        if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                            return;

                        if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                            return;

                        SocketReaction[] reactions = data.Reactions.Select(x => SocketReaction.Create(_client, ToReactionData(data, x), x.Member == null ? null : guild.AddOrUpdateMember(x.Member))).ToArray();
                        MessageReactionsAdded?.Invoke(channel, new Cacheable<Message>(data.MessageId, await GetCachedMessageAsync(data.MessageId), async () =>
                        {
                            return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                        }), reactions);
                    }
                    else
                    {
                        Channels.TryGetValue(data.ChannelId, out Channel channel);
                        PrivateMessageReactionsAdded?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                        {
                            Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                            if (channel == null)
                                return null;
                            Channels.TryAdd(data.ChannelId, channel);

                            return channel;
                        }), new Cacheable<Message>(data.MessageId, null, async () =>
                        {
                            return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                        }), data.Reactions.Select(x => SocketReaction.Create(_client, ToReactionData(data, x), null)).ToArray());
                    }
                }
                return;
            case "MESSAGE_REACTION_REMOVE":
                {
                    MessageReactionGatewayData? data = p.Data.ToObject<MessageReactionGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            SocketGuildMember member = guild.AddOrUpdateMember(data.Member);

                            MessageReactionRemoved?.Invoke(channel, new Cacheable<Message>(data.MessageId, await GetCachedMessageAsync(data.MessageId), async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }), SocketReaction.Create(_client, data, member));
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageReactionRemoved?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), new Cacheable<Message>(data.MessageId, null, async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }), SocketReaction.Create(_client, data, null));
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_REACTION_REMOVE event received but data could not be cast to MessageReactionGatewayData");
                }
                return;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    MessageReactionsRemoveGatewayData? data = p.Data.ToObject<MessageReactionsRemoveGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            MessageReactionRemoveAll?.Invoke(channel, new Cacheable<Message>(data.MessageId, await GetCachedMessageAsync(data.MessageId), async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }));
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageReactionRemoveAll?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), new Cacheable<Message>(data.MessageId, null, async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }));
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_REACTION_REMOVE_ALL event received but data could not be cast to MessageReactionsRemovedGatewayData");
                }
                return;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    MessageReactionRemoveEmojiGatewayData? data = p.Data.ToObject<MessageReactionRemoveEmojiGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            MessageReactionRemovedEmoji?.Invoke(channel, new Cacheable<Message>(data.MessageId, await GetCachedMessageAsync(data.MessageId), async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }), GuildEmoji.Create(_client, data.Emoji));
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateMessageReactionRemovedEmoji?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), new Cacheable<Message>(data.MessageId, null, async () =>
                            {
                                return await _client.Rest.GetMessageAsync(data.ChannelId, data.MessageId);
                            }), GuildEmoji.Create(_client, data.Emoji));
                        }
                    }
                    else
                        _logger.Warning("MESSAGE_REACTION_REMOVE_EMOJI event received but data could not be cast to MessageReactionRemoveEmojiGatewayData");
                }
                return;

            // Typing
            case "TYPING_START":
                {
                    TypingGatewayData? data = p.Data.ToObject<TypingGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            TypingStarted?.Invoke(channel, guild, data.UserId, data.TimestampAt);
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateTypingStarted?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }), data.UserId, data.TimestampAt);
                        }
                    }
                    else
                        _logger.Warning("TYPING_START event received but data could not be cast to TypingGatewayData");
                }
                return;

            // Pins
            case "CHANNEL_PINS_UPDATE":
                {
                    ChannelPinsUpdateGatewayData? data = p.Data.ToObject<ChannelPinsUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (data.GuildId.HasValue)
                        {
                            if (!Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                                return;

                            if (!Channels.TryGetValue(data.ChannelId, out Channel channel))
                                return;

                            DateTimeOffset? pinBefore = channel.LastPinAt;
                            channel.LastPinAt = data.LastPinAt;
                            ChannelPinsUpdated?.Invoke(channel, guild);
                            ChannelPinsChanged?.Invoke(channel, guild, pinBefore);
                        }
                        else
                        {
                            Channels.TryGetValue(data.ChannelId, out Channel channel);
                            PrivateChannelPinsUpdated?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                            {
                                Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                                if (channel == null)
                                    return null;
                                Channels.TryAdd(data.ChannelId, channel);

                                return channel;
                            }));
                        }
                    }
                    else
                        _logger.Warning("CHANNEL_PINS_UPDATE event received but data could not be cast to ChannelPinsUpdateGatewayData");
                }
                return;
            case "CHANNEL_PINS_ACK":
                {
                    ChannelPinsAckGatewayData? data = p.Data.ToObject<ChannelPinsAckGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        Channels.TryGetValue(data.ChannelId, out Channel channel);
                        ChannelPinsAck?.Invoke(new Cacheable<Channel>(data.ChannelId, channel, async () =>
                        {
                            Channel? channel = await _client.Rest.GetChannelAsync(data.ChannelId);
                            if (channel == null)
                                return null;
                            if (channel.Type == ChannelType.DM || channel.Type == ChannelType.Group)
                                Channels.TryAdd(data.ChannelId, channel);

                            return channel;
                        }));
                    }
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
                        SocketVoiceState? previousState = null;
                        if (data.GuildId.HasValue && Guilds.TryGetValue(data.GuildId.Value, out SocketGuild guild))
                        {
                            guild.AddOrUpdateMember(data.Member);
                            SocketGuildMember member = guild.Members.GetValueOrDefault(data.Member.Id);
                            if (member.VoiceStates.TryGetValue(data.SessionId, out SocketVoiceState cachedState))
                                previousState = (SocketVoiceState)cachedState.Snapshot();

                            if (data.ChannelId.HasValue)
                            {
                                SocketVoiceChannel? Channel = GetChannel(data.ChannelId.Value) as SocketVoiceChannel;
                                if (member.VoiceStates.TryGetValue(data.SessionId, out SocketVoiceState state))
                                {
                                    ulong? previousChannelId = state.ChannelId;
                                    state.Update(data);
                                    if (previousChannelId != data.ChannelId)
                                    {
                                        if (previousChannelId.HasValue && GetChannel(previousChannelId.Value) is SocketVoiceChannel previousChannel)
                                            previousChannel.VoiceStates.TryRemove(data.SessionId, out _);
                                        if (Channel != null)
                                            Channel.VoiceStates[data.SessionId] = state;
                                    }
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
                        if (data.GuildId.HasValue)
                            VoiceStateChanged?.Invoke(previousState, currentState);
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
                    CallGatewayData? data = p.Data.ToObject<CallGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            CallCreated?.Invoke(SocketCall.Create(_client, data, channel));
                    }
                    else
                        _logger.Warning("CALL_CREATE event received but data could not be cast to CallGatewayData");
                }
                return;
            case "CALL_UPDATE":
                {
                    CallGatewayData? data = p.Data.ToObject<CallGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            CallUpdated?.Invoke(SocketCall.Create(_client, data, channel));
                    }
                    else
                        _logger.Warning("CALL_UPDATE event received but data could not be cast to CallGatewayData");
                }
                return;
            case "CALL_DELETE":
                {
                    CallDeleteGatewayData? data = p.Data.ToObject<CallDeleteGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        if (Channels.TryGetValue(data.ChannelId, out Channel channel))
                            CallDeleted?.Invoke(channel, data.IsUnavailable.GetValueOrDefault());
                    }
                    else
                        _logger.Warning("CALL_DELETE event received but data could not be cast to CallGatewayData");
                }
                return;

            // Counts
            case "GUILD_COUNTS_UPDATE":
                {
                    CountGatewayData<GuildMemberCountGatewayData> data = p.Data.ToObject<CountGatewayData<GuildMemberCountGatewayData>>(FluxerClient._gatewaySerializer);
                    if (data != null)
                    {
                        foreach (GuildMemberCountGatewayData count in data.Counts)
                        {
                            if (Guilds.TryGetValue(count.GuildId, out SocketGuild guild))
                            {
                                guild.MemberCount = count.MemberCount;
                                guild.OnlineCount = count.OnlineCount;
                            }
                        }

                        GuildMemberCounts?.Invoke(data.Counts);
                    }
                    else
                        _logger.Warning("GUILD_COUNTS_UPDATE event received but data could not be cast to GuildMemberCountGatewayData");
                }
                break;
            case "GUILD_HEALTH_UPDATE":
                {
                    GuildHealthUpdateGatewayData? data = p.Data.ToObject<GuildHealthUpdateGatewayData>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        GuildHealthUpdated?.Invoke(data.GuildId, data.IsDegraded);
                    else
                        _logger.Warning("GUILD_HEALTH_UPDATE event received but data could not be cast to GuildHealthUpdateGatewayData");
                }
                break;
            case "CHANNEL_MEMBER_COUNTS_UPDATE":
                {
                    CountGatewayData<GuildChannelMemberCountGatewayData> data = p.Data.ToObject<CountGatewayData<GuildChannelMemberCountGatewayData>>(FluxerClient._gatewaySerializer);
                    if (data != null)
                        ChannelMemberCounts?.Invoke(data.Counts);
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
            _sendQueue.ResetConnection();
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

                packet = CreateIdentifyPacket(new Dictionary<string, string>
                {
                    { "os", Environment.OSVersion.Platform.ToString() },
                    { "browser", "Fluxer.Net" },
                    { "device", "Fluxer.Net" }
                });
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
        long sentAt = Interlocked.Read(ref _heartbeatSentAt);
        if (sentAt != 0)
            Latency = (int)((Stopwatch.GetTimestamp() - sentAt) * 1000 / Stopwatch.Frequency);
        HeartbeatAck?.Invoke();
    }

    /// <summary>
    /// Handles Fluxer-specific gateway close codes and determines appropriate action.
    /// </summary>
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
                Interlocked.Exchange(ref _heartbeatSentAt, Stopwatch.GetTimestamp());
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
    /// <remarks>
    /// This sends a PRESENCE_UPDATE packet to the gateway. Other users will see the status change
    /// in real-time through PRESENCE_UPDATE events.
    /// </remarks>
    public void SetStatus(Status? status = null, bool? mobile = null, UserCustomStatusJson? custom = null)
    {
        PresenceUpdateGatewayData presence;
        lock (_presenceLock)
        {
            CurrentStatus = status ?? CurrentStatus;
            _presenceMobile = mobile ?? _presenceMobile;
            CurrentCustomStatus = custom ?? CurrentCustomStatus;
            presence = new PresenceUpdateGatewayData(CurrentStatus, _presenceMobile, CurrentCustomStatus);
        }

        SendGatewayPacket(new GatewayPacket
        {
            Data = JToken.FromObject(presence),
            OpCode = FluxerOpCode.PresenceUpdate
        });
    }

    /// <summary>
    /// Sets the custom status text and keeps the online status. Null or empty clears the custom status.
    /// </summary>
    public void SetCustomStatus(string? text, string? emojiName = null)
    {
        PresenceUpdateGatewayData presence;
        lock (_presenceLock)
        {
            CurrentCustomStatus = string.IsNullOrWhiteSpace(text) && string.IsNullOrEmpty(emojiName)
                ? null
                : new UserCustomStatusJson { Text = text, EmojiName = emojiName };
            presence = new PresenceUpdateGatewayData(CurrentStatus, _presenceMobile, CurrentCustomStatus ?? new UserCustomStatusJson());
        }

        SendGatewayPacket(new GatewayPacket
        {
            Data = JToken.FromObject(presence),
            OpCode = FluxerOpCode.PresenceUpdate
        });
    }

    private readonly object _presenceLock = new object();
    private bool? _presenceMobile;

    internal void SeedPresence(PresenceUpdateGatewayData? presence)
    {
        if (presence == null)
            return;

        lock (_presenceLock)
        {
            CurrentStatus ??= presence.Status switch
            {
                "online" => Status.Online,
                "idle" => Status.Idle,
                "dnd" => Status.Dnd,
                "invisible" => Status.Invisible,
                _ => null
            };
            _presenceMobile ??= presence.IsMobile;
            CurrentCustomStatus ??= presence.CustomStatus;
        }
    }

    /// <summary>
    /// The online status last set with <see cref="SetStatus"/>, or null when none was set.
    /// </summary>
    public Status? CurrentStatus { get; private set; }

    /// <summary>
    /// The custom status last set with <see cref="SetStatus"/> or <see cref="SetCustomStatus"/>, or null when there is none.
    /// </summary>
    public UserCustomStatusJson? CurrentCustomStatus { get; private set; }

    /// <summary>
    /// Sends a Lazy Request with what the session wants to receive from each guild.
    /// </summary>
    /// <remarks>
    /// Fluxer sends a guild member's presence updates only to sessions that listed them in <see cref="GuildSubscription.Members"/>.
    /// Fluxer applies each guild's options again when the session connects to that guild later.
    /// </remarks>
    /// <param name="subscriptions">The options for each guild, keyed by guild ID.</param>
    public void SendLazyRequest(IDictionary<ulong, GuildSubscription> subscriptions)
    {
        SendGatewayPacket(new GatewayPacket
        {
            Data = JToken.FromObject(new LazyRequestPacket { Subscriptions = subscriptions }, FluxerClient._gatewaySerializer),
            OpCode = FluxerOpCode.LazyRequest
        });
    }

    /// <summary>
    /// Subscribes to the presence updates of up to 1,000 members of a guild, replacing the guild's previous list.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    /// <param name="userIds">The members to follow. Only the first 1,000 are sent.</param>
    public void SubscribePresences(ulong guildId, IEnumerable<ulong> userIds)
        => SendLazyRequest(new Dictionary<ulong, GuildSubscription>
        {
            [guildId] = new GuildSubscription { Members = userIds.Distinct().Take(1000).ToArray() }
        });

    /// <summary>
    /// Asks for unread counts of up to 40 posts in a forum or media channel. The answer arrives as
    /// <see cref="ForumUnreadsReceived"/>. Fluxer accepts five of these every 5 seconds.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    /// <param name="channelId">The forum or media channel.</param>
    /// <param name="threads">The posts to count.</param>
    public void RequestForumUnreads(ulong guildId, ulong channelId, IEnumerable<ForumUnreadRequest> threads)
    {
        SendGatewayPacket(new GatewayPacket
        {
            Data = JToken.FromObject(new ForumUnreadsRequestPacket
            {
                GuildId = guildId,
                ChannelId = channelId,
                Threads = threads.Take(40).ToArray()
            }, FluxerClient._gatewaySerializer),
            OpCode = FluxerOpCode.RequestForumUnreads
        });
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
    public GatewayEvent HeartbeatAck { get => Event<GatewayEvent>(); set => Keep(value); }

    /// <summary>
    /// Delegate for gateway connected.
    /// </summary>
    public delegate void ConnectedEvent();

    /// <summary>
    /// Occurs when the gateway is connected.
    /// </summary>
    public GatewayEvent Connected { get => Event<GatewayEvent>(); set => Keep(value); }

    /// <summary>
    /// Delegate for gateway disconnected.
    /// </summary>
    public delegate void DisconnectedEvent();

    /// <summary>
    /// Occurs when the gateway is disconnected.
    /// </summary>
    public GatewayEvent Disconnected { get => Event<GatewayEvent>(); set => Keep(value); }

    /// <summary>
    /// Delegate for the READY event fired when the gateway connection is established.
    /// </summary>
    public delegate void ReadyEvent(SocketCurrentUser currentUser);

    /// <summary>
    /// Occurs when the gateway connection is established and initial data is received.
    /// Contains session ID, current user information, and initial guilds/channels/DMs.
    /// </summary>
    public GatewayEvent<SocketCurrentUser> Ready { get => Event<GatewayEvent<SocketCurrentUser>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for the RESUMED event fired when a session is successfully resumed.
    /// </summary>
    public delegate void ResumedEvent();

    /// <summary>
    /// Occurs when a disconnected session is successfully resumed without data loss.
    /// </summary>
    public GatewayEvent Resumed { get => Event<GatewayEvent>(); set => Keep(value); }

    /// <summary>
    /// Delegate for SESSIONS_REPLACE event when auth sessions are replaced.
    /// </summary>
    public delegate void SessionsReplacedEvent(GatewaySession[] sessions);

    /// <summary>
    /// Occurs when auth sessions are replaced.
    /// </summary>
    public GatewayEvent<GatewaySession[]> SessionsReplaced { get => Event<GatewayEvent<GatewaySession[]>>(); set => Keep(value); }

    // ============================================================================
    // User Settings Events
    // ============================================================================

    /// <summary>
    /// Delegate for USER_SETTINGS_UPDATE events when user settings are updated.
    /// </summary>
    public delegate void UserSettingsUpdatedEvent(UserSettings settings);

    /// <summary>
    /// Occurs when user settings are updated.
    /// </summary>
    public GatewayEvent<UserSettings> UserSettingsUpdated { get => Event<GatewayEvent<UserSettings>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for USER_GUILD_SETTINGS_UPDATE events when user guild settings are updated.
    /// </summary>
    public delegate void UserGuildSettingsUpdatedEvent(UserGuildSettings settings);

    /// <summary>
    /// Occurs when user guild settings are updated.
    /// </summary>
    public GatewayEvent<UserGuildSettings> UserGuildSettingsUpdated { get => Event<GatewayEvent<UserGuildSettings>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for USER_PINNED_DMS_UPDATE events when pinned DMs are updated.
    /// </summary>
    public delegate void UserPinnedDMsUpdatedEvent(List<ulong> pins);

    /// <summary>
    /// Occurs when pinned DMs are updated.
    /// </summary>
    public GatewayEvent<List<ulong>> UserPinnedDMsUpdated { get => Event<GatewayEvent<List<ulong>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for USER_NOTE_UPDATE events when a user note is updated.
    /// </summary>
    public delegate void UserNoteUpdatedEvent(ulong userId, string note);

    /// <summary>
    /// Occurs when a user note is updated.
    /// </summary>
    public GatewayEvent<ulong, string> UserNoteUpdated { get => Event<GatewayEvent<ulong, string>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for AUTH_SESSION_CHANGE events when an auth session changes.
    /// </summary>
    public delegate void AuthSessionChangedEvent(AuthSessionChangeGatewayData session);

    /// <summary>
    /// Occurs when an auth session changes.
    /// </summary>
    public GatewayEvent<AuthSessionChangeGatewayData> AuthSessionChanged { get => Event<GatewayEvent<AuthSessionChangeGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for RATE_LIMITED event when rate limited.
    /// </summary>
    public delegate void RateLimitedEvent(RateLimitedGatewayData ratelimited);

    /// <summary>
    /// Occurs on rate limited.
    /// </summary>
    public GatewayEvent<RateLimitedGatewayData> RateLimited { get => Event<GatewayEvent<RateLimitedGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for the gateway error opcode, sent when Fluxer refuses a voice state update.
    /// </summary>
    public delegate void GatewayErrorEvent(GatewayErrorGatewayData error);

    /// <summary>
    /// Occurs when Fluxer refuses a voice state update with a code the client is expected to act on.
    /// </summary>
    public GatewayEvent<GatewayErrorGatewayData> GatewayErrorReceived { get => Event<GatewayEvent<GatewayErrorGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for USER_CONNECTIONS_UPDATE event when user updates a connection.
    /// </summary>
    public delegate void UserConnectionsUpdatedEvent(UserConnection[] connections);

    /// <summary>
    /// Occurs on user connection updated.
    /// </summary>
    public GatewayEvent<UserConnection[]> ConnectionsUpdated { get => Event<GatewayEvent<UserConnection[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for WEBAUTHN_CREDENTIALS_UPDATE event when user updates webauthn credentials.
    /// </summary>
    public delegate void UserWebAuthnCredentialsUpdatedEvent(WebAuthnCredential[] credentials);

    /// <summary>
    /// Occurs on user connection updated.
    /// </summary>
    public GatewayEvent<WebAuthnCredential[]> WebAuthnCredentialsUpdated { get => Event<GatewayEvent<WebAuthnCredential[]>>(); set => Keep(value); }

    // ============================================================================
    // Message Events
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_CREATE events when a new message is sent.
    /// </summary>
    public delegate void MessageCreatedEvent(SocketMessage message);

    /// <summary>
    /// Occurs when a new message is created in any channel the user has access to.
    /// </summary>
    public GatewayEvent<SocketMessage> MessageCreated { get => Event<GatewayEvent<SocketMessage>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_CREATE events when a new message is sent.
    /// </summary>
    public delegate void PrivateMessageCreatedEvent(SocketPrivateMessage message);

    /// <summary>
    /// Occurs when a new message is created in any channel the user has access to.
    /// </summary>
    public GatewayEvent<SocketPrivateMessage> PrivateMessageCreated { get => Event<GatewayEvent<SocketPrivateMessage>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_UPDATE events when a message is edited.
    /// </summary>
    public delegate void MessageUpdatedEvent(SocketMessage message);

    /// <summary>
    /// Occurs when a message is edited.
    /// </summary>
    public GatewayEvent<SocketMessage> MessageUpdated { get => Event<GatewayEvent<SocketMessage>>(); set => Keep(value); }

    /// <summary>
    /// Occurs when a new message is created in any channel the user has access to.
    /// </summary>
    public GatewayEvent<SocketPrivateMessage> PrivateMessageUpdated { get => Event<GatewayEvent<SocketPrivateMessage>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_DELETE events when a message is deleted.
    /// </summary>
    public delegate void MessageDeletedEvent(Channel channel, ulong? userId, ulong messageId, string? content);

    /// <summary>
    /// Occurs when a message is deleted.
    /// </summary>
    public GatewayEvent<Channel, ulong?, ulong, string?> MessageDeleted { get => Event<GatewayEvent<Channel, ulong?, ulong, string?>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_DELETE events when a message is deleted.
    /// </summary>
    public delegate void PrivateMessageDeletedEvent(Cacheable<Channel> channel, ulong? userId, ulong messageId, string? content);

    /// <summary>
    /// Occurs when a message is deleted.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, ulong?, ulong, string?> PrivateMessageDeleted { get => Event<GatewayEvent<Cacheable<Channel>, ulong?, ulong, string?>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_UPDATE events that also hands over the message as it was before the edit.
    /// </summary>
    /// <param name="before">The cached message from before the edit, or null when it wasn't cached.</param>
    /// <param name="after">The edited message.</param>
    public delegate void MessageEditedEvent(SocketMessage? before, SocketMessage after);

    /// <summary>
    /// Occurs when a guild message is edited, after <see cref="MessageUpdated"/>.
    /// Turn on <see cref="FluxerCacheConfig.Messages"/> to get the message from before the edit.
    /// </summary>
    public GatewayEvent<SocketMessage?, SocketMessage> MessageEdited { get => Event<GatewayEvent<SocketMessage?, SocketMessage>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_DELETE events that also hands over the deleted message when it was cached.
    /// </summary>
    /// <param name="channel">The channel the message was in.</param>
    /// <param name="messageId">The ID of the deleted message.</param>
    /// <param name="message">The cached message, or null when it wasn't cached.</param>
    public delegate void MessageRemovedEvent(Channel channel, ulong messageId, SocketMessage? message);

    /// <summary>
    /// Occurs when a guild message is deleted, after <see cref="MessageDeleted"/>.
    /// Turn on <see cref="FluxerCacheConfig.Messages"/> to get the deleted message.
    /// </summary>
    public GatewayEvent<Channel, ulong, SocketMessage?> MessageRemoved { get => Event<GatewayEvent<Channel, ulong, SocketMessage?>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_DELETE_BULK events that also hands over the deleted messages that were cached.
    /// </summary>
    /// <param name="channel">The channel the messages were in.</param>
    /// <param name="messageIds">The IDs of every deleted message.</param>
    /// <param name="messages">The deleted messages that were cached.</param>
    public delegate void MessagesRemovedEvent(Channel channel, IReadOnlyList<ulong> messageIds, IReadOnlyList<SocketMessage> messages);

    /// <summary>
    /// Occurs when guild messages are bulk deleted, after <see cref="MessagesDeleted"/>.
    /// Turn on <see cref="FluxerCacheConfig.Messages"/> to get the deleted messages.
    /// </summary>
    public GatewayEvent<Channel, IReadOnlyList<ulong>, IReadOnlyList<SocketMessage>> MessagesRemoved { get => Event<GatewayEvent<Channel, IReadOnlyList<ulong>, IReadOnlyList<SocketMessage>>>(); set => Keep(value); }


    // ============================================================================
    // Channel Events
    // ============================================================================

    /// <summary>
    /// Delegate for CHANNEL_CREATE events when a channel is created.
    /// </summary>
    public delegate void ChannelCreatedEvent(SocketGuild guild, Channel channel);

    /// <summary>
    /// Occurs when a channel is created in a guild.
    /// </summary>
    public GatewayEvent<SocketGuild, Channel> ChannelCreated { get => Event<GatewayEvent<SocketGuild, Channel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_CREATE events when a channel is created.
    /// </summary>
    public delegate void DMChannelCreatedEvent(DMChannel channel);

    /// <summary>
    /// Occurs when a private DM channel is created.
    /// </summary>
    public GatewayEvent<DMChannel> DMCreated { get => Event<GatewayEvent<DMChannel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_CREATE events when a channel is created.
    /// </summary>
    public delegate void GroupChannelJoinedEvent(GroupChannel channel);

    /// <summary>
    /// Occurs when a private Group channel is joined.
    /// </summary>
    public GatewayEvent<GroupChannel> GroupJoined { get => Event<GatewayEvent<GroupChannel>>(); set => Keep(value); }

    /// <summary>
    /// Occurs when a private Group channel is left.
    /// </summary>
    public GatewayEvent<GroupChannel> GroupLeft { get => Event<GatewayEvent<GroupChannel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_UPDATE events when a channel is updated.
    /// </summary>
    public delegate void ChannelUpdatedEvent(SocketGuild guild, Channel before, Channel after);

    /// <summary>
    /// Occurs when a channel is updated.
    /// </summary>
    public GatewayEvent<SocketGuild, Channel, Channel> ChannelUpdated { get => Event<GatewayEvent<SocketGuild, Channel, Channel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_UPDATE events when a channel is updated.
    /// </summary>
    public delegate void GroupChannelUpdatedEvent(GroupChannel channel);

    /// <summary>
    /// Occurs when a private DM or Group channel is created.
    /// </summary>
    public GatewayEvent<GroupChannel> GroupUpdated { get => Event<GatewayEvent<GroupChannel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_DELETE events when a channel is deleted.
    /// </summary>
    public delegate void ChannelDeletedEvent(SocketGuild guild, Channel channel);

    /// <summary>
    /// Occurs when a channel is deleted.
    /// </summary>
    public GatewayEvent<SocketGuild, Channel> ChannelDeleted { get => Event<GatewayEvent<SocketGuild, Channel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_CREATE events when a thread is created.
    /// </summary>
    public delegate void ThreadCreatedEvent(SocketGuild guild, ThreadChannel channel);

    public GatewayEvent<SocketGuild, ThreadChannel> ThreadCreated { get => Event<GatewayEvent<SocketGuild, ThreadChannel>>(); set => Keep(value); }


    public GatewayEvent<SocketGuild, ThreadChannel> ThreadJoined { get => Event<GatewayEvent<SocketGuild, ThreadChannel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_CREATE events when a thread is created.
    /// </summary>
    public delegate void ThreadLeftEvent(SocketGuild guild, Cacheable<ThreadChannel> channel);

    public GatewayEvent<SocketGuild, Cacheable<ThreadChannel>> ThreadLeft { get => Event<GatewayEvent<SocketGuild, Cacheable<ThreadChannel>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_UPDATE events when a thread is updated.
    /// </summary>
    public delegate void ThreadUpdatedEvent(SocketGuild guild, Cacheable<ThreadChannel> before, ThreadChannel after);

    public GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, ThreadChannel> ThreadUpdated { get => Event<GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, ThreadChannel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_UPDATE events that hands over the cached thread from before the change.
    /// </summary>
    /// <param name="guild">The guild of the thread.</param>
    /// <param name="before">A copy of the cached thread from before the update, or null when it wasn't cached.</param>
    /// <param name="after">The cached thread after the update.</param>
    public delegate void ThreadChangedEvent(SocketGuild guild, Channel? before, Channel after);

    /// <summary>
    /// Occurs when a thread is updated, after <see cref="ThreadUpdated"/>.
    /// </summary>
    public GatewayEvent<SocketGuild, Channel?, Channel> ThreadChanged { get => Event<GatewayEvent<SocketGuild, Channel?, Channel>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_DELETE events when a thread is deleted.
    /// </summary>
    public delegate void ThreadDeletedEvent(SocketGuild guild, Cacheable<ThreadChannel> thread);

    public GatewayEvent<SocketGuild, Cacheable<ThreadChannel>> ThreadDeleted { get => Event<GatewayEvent<SocketGuild, Cacheable<ThreadChannel>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_DELETE events when a thread is deleted.
    /// </summary>
    public delegate void ThreadMembersAddedEvent(SocketGuild guild, Cacheable<ThreadChannel> thread, ThreadMember[] members);

    public GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, ThreadMember[]> ThreadMembersAdded { get => Event<GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, ThreadMember[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_DELETE events when a thread is deleted.
    /// </summary>
    public delegate void ThreadMembersRemovedEvent(SocketGuild guild, Cacheable<ThreadChannel> thread, ulong[] members);

    public GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, ulong[]> ThreadMembersRemoved { get => Event<GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, ulong[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_DELETE events when a thread is deleted.
    /// </summary>
    public delegate void ThreadMembersCountEvent(SocketGuild guild, Cacheable<ThreadChannel> thread, int membersCount);

    public GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, int> ThreadMembersCount { get => Event<GatewayEvent<SocketGuild, Cacheable<ThreadChannel>, int>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_LIST_SYNC events carrying the threads the gateway currently lists for a guild.
    /// </summary>
    /// <param name="guild">The guild the threads belong to.</param>
    /// <param name="threads">The threads in the sync.</param>
    /// <param name="members">The current user's thread member entries for those threads.</param>
    /// <param name="parentChannelIds">The parent channels the sync is limited to, or null when it covers the whole guild.</param>
    public delegate void ThreadListSyncedEvent(SocketGuild guild, ThreadChannel[] threads, ThreadMember[] members, ulong[]? parentChannelIds);

    /// <summary>
    /// Occurs when the gateway syncs the thread list of a guild.
    /// </summary>
    public GatewayEvent<SocketGuild, ThreadChannel[], ThreadMember[], ulong[]?> ThreadListSynced { get => Event<GatewayEvent<SocketGuild, ThreadChannel[], ThreadMember[], ulong[]?>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_MEMBER_UPDATE events when the current user's membership of a thread changes.
    /// </summary>
    public delegate void ThreadMemberUpdatedEvent(SocketGuild guild, ThreadMember member);

    /// <summary>
    /// Occurs when the current user's membership of a thread changes, for example its flags or mute state.
    /// </summary>
    public GatewayEvent<SocketGuild, ThreadMember> ThreadMemberUpdated { get => Event<GatewayEvent<SocketGuild, ThreadMember>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for THREAD_MEMBER_LIST_UPDATE events.
    /// </summary>
    public delegate void ThreadMemberListUpdatedEvent(ThreadMemberListUpdateGatewayData data);

    /// <summary>
    /// Occurs with the full member list of a thread subscribed to with <see cref="GuildSubscription.ThreadMemberLists"/>.
    /// </summary>
    public GatewayEvent<ThreadMemberListUpdateGatewayData> ThreadMemberListUpdated { get => Event<GatewayEvent<ThreadMemberListUpdateGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for FORUM_UNREADS events.
    /// </summary>
    public delegate void ForumUnreadsReceivedEvent(ForumUnreadsGatewayData data);

    /// <summary>
    /// Occurs with the answer to <see cref="RequestForumUnreads"/>.
    /// </summary>
    public GatewayEvent<ForumUnreadsGatewayData> ForumUnreadsReceived { get => Event<GatewayEvent<ForumUnreadsGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_MEMBER_LIST_UPDATE events.
    /// </summary>
    public delegate void GuildMemberListUpdatedEvent(GuildMemberListUpdateGatewayData data);

    /// <summary>
    /// Occurs when a member list subscribed to with <see cref="GuildSubscription.MemberListChannels"/> changes.
    /// </summary>
    public GatewayEvent<GuildMemberListUpdateGatewayData> GuildMemberListUpdated { get => Event<GatewayEvent<GuildMemberListUpdateGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_HEALTH_UPDATE events when a guild becomes degraded or recovers.
    /// </summary>
    public delegate void GuildHealthUpdatedEvent(ulong guildId, bool isDegraded);

    /// <summary>
    /// Occurs when the gateway reports a guild as degraded or recovered.
    /// </summary>
    public GatewayEvent<ulong, bool> GuildHealthUpdated { get => Event<GatewayEvent<ulong, bool>>(); set => Keep(value); }

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
    public GatewayEvent<SocketCurrentUser, SocketCurrentUser> CurrentUserUpdated { get => Event<GatewayEvent<SocketCurrentUser, SocketCurrentUser>>(); set => Keep(value); }

    // ============================================================================
    // Presence Events
    // ============================================================================

    /// <summary>
    /// Delegate for PRESENCE_UPDATE events when a user's presence changes.
    /// </summary>
    public delegate void PresenceUpdatedEvent(PresenceGatewayData presence);

    /// <summary>
    /// Occurs when a user's presence (online status) changes.
    /// </summary>
    public GatewayEvent<PresenceGatewayData> PresenceUpdated { get => Event<GatewayEvent<PresenceGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for PRESENCE_UPDATE events that also hands over the user's previous presence.
    /// </summary>
    /// <param name="before">The cached presence from before, or null when it wasn't cached.</param>
    /// <param name="after">The new presence.</param>
    public delegate void PresenceChangedEvent(PresenceGatewayData? before, PresenceGatewayData after);

    /// <summary>
    /// Occurs when a presence changes, after <see cref="PresenceUpdated"/>. Turn on <see cref="FluxerCacheConfig.Presences"/>
    /// to get the previous presence.
    /// </summary>
    public GatewayEvent<PresenceGatewayData?, PresenceGatewayData> PresenceChanged { get => Event<GatewayEvent<PresenceGatewayData?, PresenceGatewayData>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for user changes seen in member, presence and message events.
    /// </summary>
    /// <param name="before">The user as cached before the change.</param>
    /// <param name="after">The user now.</param>
    public delegate void UserChangedEvent(User before, User after);

    /// <summary>
    /// Occurs when a user's name, discriminator, display name, avatar or flags change. Needs <see cref="FluxerCacheConfig.Users"/>,
    /// and is raised the first time the gateway sees the user again after the change.
    /// </summary>
    public GatewayEvent<User, User> UserChanged { get => Event<GatewayEvent<User, User>>(); set => Keep(value); }

    // ============================================================================
    // Typing Events
    // ============================================================================

    /// <summary>
    /// Delegate for TYPING_START events when a user starts typing.
    /// </summary>
    public delegate void TypingStartEvent(Channel channel, SocketGuild? guild, ulong userId, DateTimeOffset timestamp);

    /// <summary>
    /// Occurs when a user starts typing in a channel.
    /// </summary>
    public GatewayEvent<Channel, SocketGuild?, ulong, DateTimeOffset> TypingStarted { get => Event<GatewayEvent<Channel, SocketGuild?, ulong, DateTimeOffset>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for TYPING_START events when a user starts typing.
    /// </summary>
    public delegate void PrivateTypingStartEvent(Cacheable<Channel> channel, ulong userId, DateTimeOffset timestamp);

    /// <summary>
    /// Occurs when a user starts typing in a channel.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, ulong, DateTimeOffset> PrivateTypingStarted { get => Event<GatewayEvent<Cacheable<Channel>, ulong, DateTimeOffset>>(); set => Keep(value); }

    // ============================================================================
    // Message Reaction Events
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_REACTION_ADD events when a reaction is added.
    /// </summary>
    public delegate void MessageReactionAddedEvent(Channel channel, Cacheable<Message> message, SocketReaction reaction);

    /// <summary>
    /// Occurs when a user adds a reaction to a message.
    /// </summary>
    public GatewayEvent<Channel, Cacheable<Message>, SocketReaction> MessageReactionAdded { get => Event<GatewayEvent<Channel, Cacheable<Message>, SocketReaction>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_ADD events when a reaction is added.
    /// </summary>
    public delegate void PrivateMessageReactionAddedEvent(Cacheable<Channel> channel, Cacheable<Message> message, SocketReaction reaction);

    /// <summary>
    /// Occurs when a user adds a reaction to a message.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, Cacheable<Message>, SocketReaction> PrivateMessageReactionAdded { get => Event<GatewayEvent<Cacheable<Channel>, Cacheable<Message>, SocketReaction>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_ADD_MANY events when several reactions are added to a message at once.
    /// </summary>
    public delegate void MessageReactionsAddedEvent(Channel channel, Cacheable<Message> message, SocketReaction[] reactions);

    /// <summary>
    /// Occurs when the gateway delivers a batch of reactions added to a message. Needs <see cref="FluxerGatewayConfig.DebounceMessageReactions"/>.
    /// </summary>
    public GatewayEvent<Channel, Cacheable<Message>, SocketReaction[]> MessageReactionsAdded { get => Event<GatewayEvent<Channel, Cacheable<Message>, SocketReaction[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_ADD_MANY events in direct messages.
    /// </summary>
    public delegate void PrivateMessageReactionsAddedEvent(Cacheable<Channel> channel, Cacheable<Message> message, SocketReaction[] reactions);

    /// <summary>
    /// Occurs when the gateway delivers a batch of reactions added to a direct message. Needs <see cref="FluxerGatewayConfig.DebounceMessageReactions"/>.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, Cacheable<Message>, SocketReaction[]> PrivateMessageReactionsAdded { get => Event<GatewayEvent<Cacheable<Channel>, Cacheable<Message>, SocketReaction[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE events when a reaction is removed.
    /// </summary>
    public delegate void MessageReactionRemovedEvent(Channel channel, Cacheable<Message> message, SocketReaction reaction);

    /// <summary>
    /// Occurs when a user removes a reaction from a message.
    /// </summary>
    public GatewayEvent<Channel, Cacheable<Message>, SocketReaction> MessageReactionRemoved { get => Event<GatewayEvent<Channel, Cacheable<Message>, SocketReaction>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE events when a reaction is removed.
    /// </summary>
    public delegate void PrivateMessageReactionRemovedEvent(Cacheable<Channel> channel, Cacheable<Message> message, SocketReaction reaction);

    /// <summary>
    /// Occurs when a user removes a reaction from a message.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, Cacheable<Message>, SocketReaction> PrivateMessageReactionRemoved { get => Event<GatewayEvent<Cacheable<Channel>, Cacheable<Message>, SocketReaction>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_ALL events when all reactions are cleared.
    /// </summary>
    public delegate void MessageReactionRemoveAllEvent(Channel channel, Cacheable<Message> message);

    /// <summary>
    /// Occurs when all reactions are removed from a message.
    /// </summary>
    public GatewayEvent<Channel, Cacheable<Message>> MessageReactionRemoveAll { get => Event<GatewayEvent<Channel, Cacheable<Message>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_ALL events when all reactions are cleared.
    /// </summary>
    public delegate void PrivateMessageReactionRemoveAllEvent(Cacheable<Channel> channel, Cacheable<Message> message);

    /// <summary>
    /// Occurs when all reactions are removed from a message.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, Cacheable<Message>> PrivateMessageReactionRemoveAll { get => Event<GatewayEvent<Cacheable<Channel>, Cacheable<Message>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_EMOJI events when all reactions of a specific emoji are removed.
    /// </summary>
    public delegate void MessageReactionRemovedEmojiEvent(Channel channel, Cacheable<Message> message, IEmoji emoji);

    /// <summary>
    /// Occurs when all instances of a specific emoji are removed from a message.
    /// </summary>
    public GatewayEvent<Channel, Cacheable<Message>, IEmoji> MessageReactionRemovedEmoji { get => Event<GatewayEvent<Channel, Cacheable<Message>, IEmoji>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_REACTION_REMOVE_EMOJI events when all reactions of a specific emoji are removed.
    /// </summary>
    public delegate void PrivateMessageReactionRemovedEmojiEvent(Cacheable<Channel> channel, Cacheable<Message> message, IEmoji emoji);

    /// <summary>
    /// Occurs when all instances of a specific emoji are removed from a message.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, Cacheable<Message>, IEmoji> PrivateMessageReactionRemovedEmoji { get => Event<GatewayEvent<Cacheable<Channel>, Cacheable<Message>, IEmoji>>(); set => Keep(value); }

    // ============================================================================
    // Saved Messages and Mentions
    // ============================================================================

    /// <summary>
    /// Delegate for SAVED_MESSAGE_CREATE events when a message is saved.
    /// </summary>
    public delegate void SavedMessageCreatedEvent(SavedMessage message);

    /// <summary>
    /// Occurs when a message is saved.
    /// </summary>
    public GatewayEvent<SavedMessage> SavedMessageCreated { get => Event<GatewayEvent<SavedMessage>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for SAVED_MESSAGE_DELETE events when a saved message is deleted.
    /// </summary>
    public delegate void SavedMessageDeletedEvent(ulong messageId);

    /// <summary>
    /// Occurs when a saved message is deleted.
    /// </summary>
    public GatewayEvent<ulong> SavedMessageDeleted { get => Event<GatewayEvent<ulong>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for RECENT_MENTION_DELETE events when a recent mention is deleted.
    /// </summary>
    public delegate void RecentMentionDeletedEvent(ulong messageId);

    /// <summary>
    /// Occurs when a recent mention is deleted.
    /// </summary>
    public GatewayEvent<ulong> RecentMentionDeleted { get => Event<GatewayEvent<ulong>>(); set => Keep(value); }

    // ============================================================================
    // Message Bulk Operations
    // ============================================================================

    /// <summary>
    /// Delegate for MESSAGE_DELETE_BULK events when multiple messages are deleted at once.
    /// </summary>
    public delegate void MessageDeleteBulkEvent(Channel channel, SocketGuild? guild, ulong[] messageIds);

    /// <summary>
    /// Occurs when multiple messages are deleted in bulk (e.g., purge operation).
    /// </summary>
    public GatewayEvent<Channel, SocketGuild?, ulong[]> MessagesDeleted { get => Event<GatewayEvent<Channel, SocketGuild?, ulong[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_DELETE_BULK events when multiple messages are deleted at once.
    /// </summary>
    public delegate void PrivateMessageDeleteBulkEvent(Cacheable<Channel> channel, ulong[] messageIds);

    /// <summary>
    /// Occurs when multiple messages are deleted in bulk (e.g., purge operation).
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, ulong[]> PrivateMessagesDeleted { get => Event<GatewayEvent<Cacheable<Channel>, ulong[]>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for MESSAGE_ACK events when a message is acknowledged as read.
    /// </summary>
    public delegate void MessageAckEvent(Cacheable<Channel> channel, Cacheable<Message> message);

    /// <summary>
    /// Occurs when a message is acknowledged as read by the current user.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>, Cacheable<Message>> MessageAck { get => Event<GatewayEvent<Cacheable<Channel>, Cacheable<Message>>>(); set => Keep(value); }

    // ============================================================================
    // Channel Update Events
    // ============================================================================

    /// <summary>
    /// Delegate for CHANNEL_PINS_UPDATE events when channel pins are updated.
    /// </summary>
    public delegate void ChannelPinsUpdatedEvent(Channel channel, SocketGuild? guild);

    /// <summary>
    /// Occurs when a message is pinned or unpinned in a channel.
    /// </summary>
    public GatewayEvent<Channel, SocketGuild?> ChannelPinsUpdated { get => Event<GatewayEvent<Channel, SocketGuild?>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_PINS_UPDATE events in guilds that also hands over the previous pin time.
    /// </summary>
    /// <param name="channel">The channel, with its new <see cref="Channel.LastPinAt"/>.</param>
    /// <param name="guild">The guild of the channel.</param>
    /// <param name="before">The channel's last pin time before the change.</param>
    public delegate void ChannelPinsChangedEvent(Channel channel, SocketGuild guild, DateTimeOffset? before);

    /// <summary>
    /// Occurs when a guild channel's pins change, after <see cref="ChannelPinsUpdated"/>.
    /// </summary>
    public GatewayEvent<Channel, SocketGuild, DateTimeOffset?> ChannelPinsChanged { get => Event<GatewayEvent<Channel, SocketGuild, DateTimeOffset?>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_PINS_UPDATE events when channel pins are updated.
    /// </summary>
    public delegate void PrivateChannelPinsUpdatedEvent(Cacheable<Channel> channel);

    /// <summary>
    /// Occurs when a message is pinned or unpinned in a channel.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>> PrivateChannelPinsUpdated { get => Event<GatewayEvent<Cacheable<Channel>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_PINS_ACK events when channel pins are acknowledged.
    /// </summary>
    public delegate void ChannelPinsAckEvent(Cacheable<Channel> channel);

    /// <summary>
    /// Occurs when channel pins are acknowledged.
    /// </summary>
    public GatewayEvent<Cacheable<Channel>> ChannelPinsAck { get => Event<GatewayEvent<Cacheable<Channel>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_RECIPIENT_ADD events when a recipient is added to a channel.
    /// </summary>
    public delegate void GroupUserAddedEvent(Cacheable<GroupChannel> channel, SocketUser user);

    /// <summary>
    /// Occurs when a recipient is added to a group DM or channel.
    /// </summary>
    public GatewayEvent<Cacheable<GroupChannel>, SocketUser> GroupUserAdded { get => Event<GatewayEvent<Cacheable<GroupChannel>, SocketUser>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CHANNEL_RECIPIENT_REMOVE events when a recipient is removed from a channel.
    /// </summary>
    public delegate void GroupUserRemovedEvent(Cacheable<GroupChannel> channel, SocketUser user);

    /// <summary>
    /// Occurs when a recipient is removed from a group DM or channel.
    /// </summary>
    public GatewayEvent<Cacheable<GroupChannel>, SocketUser> GroupUserRemoved { get => Event<GatewayEvent<Cacheable<GroupChannel>, SocketUser>>(); set => Keep(value); }

    // ============================================================================
    // Voice Events
    // ============================================================================

    /// <summary>
    /// Delegate for VOICE_STATE_UPDATE events when a user's voice state changes.
    /// </summary>
    public delegate void VoiceStateUpdatedEvent(SocketVoiceState voiceState);

    /// <summary>
    /// Occurs when a user joins, leaves, or updates their state in a voice channel.
    /// </summary>
    public GatewayEvent<SocketVoiceState> VoiceStateUpdated { get => Event<GatewayEvent<SocketVoiceState>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for VOICE_STATE_UPDATE events that also hands over the voice state from before the change.
    /// </summary>
    /// <param name="before">The cached state of that voice connection, or null when it just joined or wasn't cached.</param>
    /// <param name="after">The new state. Its channel is null when the connection left voice.</param>
    public delegate void VoiceStateChangedEvent(SocketVoiceState? before, SocketVoiceState after);

    /// <summary>
    /// Occurs when a voice connection in a guild joins, leaves, moves or changes state, after <see cref="VoiceStateUpdated"/>.
    /// </summary>
    public GatewayEvent<SocketVoiceState?, SocketVoiceState> VoiceStateChanged { get => Event<GatewayEvent<SocketVoiceState?, SocketVoiceState>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for VOICE_SERVER_UPDATE events for voice connection data.
    /// </summary>
    public delegate void VoiceServerUpdatedEvent(VoiceServer voiceServer);

    /// <summary>
    /// Occurs when voice server information is updated (used for establishing voice connections).
    /// </summary>
    public GatewayEvent<VoiceServer> VoiceServerUpdated { get => Event<GatewayEvent<VoiceServer>>(); set => Keep(value); }

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
    public GatewayEvent<SocketGuild, Cacheable<SocketGuildMember>> UserBanned { get => Event<GatewayEvent<SocketGuild, Cacheable<SocketGuildMember>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_BAN_REMOVE events when a user is unbanned from a guild.
    /// </summary>
    public delegate void GuildBanRemovedEvent(SocketGuild guild, ulong userId);

    /// <summary>
    /// Occurs when a user is unbanned from a guild.
    /// </summary>
    public GatewayEvent<SocketGuild, ulong> UserUnbanned { get => Event<GatewayEvent<SocketGuild, ulong>>(); set => Keep(value); }

    // ============================================================================
    // Webhook Events
    // ============================================================================

    /// <summary>
    /// Delegate for WEBHOOKS_UPDATE events when webhooks in a channel are updated.
    /// </summary>
    public delegate void WebhooksUpdatedEvent(SocketGuild guild, Channel channel);

    /// <summary>
    /// Occurs when webhooks are created, updated, or deleted in a channel.
    /// </summary>
    public GatewayEvent<SocketGuild, Channel> WebhooksUpdated { get => Event<GatewayEvent<SocketGuild, Channel>>(); set => Keep(value); }

    // ============================================================================
    // Guild Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_CREATE events when a guild becomes available.
    /// </summary>
    public delegate void GuildJoinedEvent(SocketGuild guild);

    /// <summary>
    /// Occurs when the bot joins a guild.
    /// </summary>
    public GatewayEvent<SocketGuild> GuildJoined { get => Event<GatewayEvent<SocketGuild>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_UPDATE events when guild properties are updated.
    /// </summary>
    public delegate void GuildUpdatedEvent(SocketGuild before, SocketGuild after);

    /// <summary>
    /// Occurs when a guild is updated (name, icon, settings, etc.).
    /// </summary>
    public GatewayEvent<SocketGuild, SocketGuild> GuildUpdated { get => Event<GatewayEvent<SocketGuild, SocketGuild>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_DELETE events when the bot is removed.
    /// </summary>
    public delegate void GuildLeftEvent(SocketGuild guild);

    /// <summary>
    /// Occurs when the bot is removed from a guild or when a guild becomes unavailable.
    /// Check the Unavailable property to distinguish between guild outages (true) and the user being removed (false/null).
    /// </summary>
    public GatewayEvent<SocketGuild> GuildLeft { get => Event<GatewayEvent<SocketGuild>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_CREATE events when a guild has come back.
    /// </summary>
    public delegate void GuildAvailableEvent(SocketGuild guild);

    /// <summary>
    /// Occurs when the guild becomes available again.
    /// </summary>
    public GatewayEvent<SocketGuild> GuildAvailable { get => Event<GatewayEvent<SocketGuild>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_DELETE events when a guild has gone down.
    /// </summary>
    public delegate void GuildUnavailableEvent(SocketGuild guild);

    /// <summary>
    /// Occurs when the guild becomes unavailable.
    /// </summary>
    public GatewayEvent<SocketGuild> GuildUnavailable { get => Event<GatewayEvent<SocketGuild>>(); set => Keep(value); }

    // ============================================================================
    // Guild Member Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_MEMBER_ADD events when a user joins a guild.
    /// </summary>
    public delegate void GuildMemberAddedEvent(SocketGuildMember member);

    /// <summary>
    /// Occurs when a new member joins a guild.
    /// </summary>
    public GatewayEvent<SocketGuildMember> MemberJoined { get => Event<GatewayEvent<SocketGuildMember>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_MEMBER_UPDATE events when a guild member is updated.
    /// </summary>
    public delegate void GuildMemberUpdatedEvent(SocketGuildMember member);

    /// <summary>
    /// Occurs when a guild member is updated (roles, nickname, avatar, etc.).
    /// </summary>
    public GatewayEvent<SocketGuildMember> MemberUpdated { get => Event<GatewayEvent<SocketGuildMember>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_MEMBER_UPDATE events that also hands over the member as they were before.
    /// </summary>
    /// <param name="before">A copy of the cached member from before the update, or null when they weren't cached.</param>
    /// <param name="after">The updated member.</param>
    public delegate void GuildMemberChangedEvent(SocketGuildMember? before, SocketGuildMember after);

    /// <summary>
    /// Occurs when a guild member is updated, after <see cref="MemberUpdated"/>.
    /// </summary>
    public GatewayEvent<SocketGuildMember?, SocketGuildMember> MemberChanged { get => Event<GatewayEvent<SocketGuildMember?, SocketGuildMember>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_MEMBER_REMOVE events when a member leaves or is removed from a guild.
    /// </summary>
    public delegate void GuildMemberRemovedEvent(SocketGuild guild, Cacheable<SocketGuildMember> member);

    /// <summary>
    /// Occurs when a member leaves a guild or is kicked/banned.
    /// </summary>
    public GatewayEvent<SocketGuild, Cacheable<SocketGuildMember>> MemberLeft { get => Event<GatewayEvent<SocketGuild, Cacheable<SocketGuildMember>>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_AUDIT_LOG_ENTRY_CREATE event.
    /// </summary>
    public delegate void AuditLogCreatedEvent(SocketGuild guild, SocketAuditLog auditLog);

    /// <summary>
    /// Occurs when an audit log is created.
    /// </summary>
    public GatewayEvent<SocketGuild, SocketAuditLog> AuditLogCreated { get => Event<GatewayEvent<SocketGuild, SocketAuditLog>>(); set => Keep(value); }

    // ============================================================================
    // Guild Role Events
    // ============================================================================

    /// <summary>
    /// Delegate for GUILD_ROLE_CREATE events when a role is created in a guild.
    /// </summary>
    public delegate void GuildRoleCreatedEvent(SocketRole role);

    /// <summary>
    /// Occurs when a new role is created in a guild.
    /// </summary>
    public GatewayEvent<SocketRole> RoleCreated { get => Event<GatewayEvent<SocketRole>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_ROLE_UPDATE events when a role is updated.
    /// </summary>
    public delegate void GuildRoleUpdatedEvent(SocketRole before, SocketRole after);

    /// <summary>
    /// Occurs when a role is updated (name, color, permissions, etc.).
    /// </summary>
    public GatewayEvent<SocketRole, SocketRole> RoleUpdated { get => Event<GatewayEvent<SocketRole, SocketRole>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_ROLE_DELETE events when a role is deleted from a guild.
    /// </summary>
    public delegate void GuildRoleDeletedEvent(SocketRole role);

    /// <summary>
    /// Occurs when a role is deleted from a guild.
    /// </summary>
    public GatewayEvent<SocketRole> RoleDeleted { get => Event<GatewayEvent<SocketRole>>(); set => Keep(value); }

    // ============================================================================
    // Guild Emoji Events
    // ============================================================================

    public delegate void GuildEmojiCreatedEvent(Emoji emoji);

    public GatewayEvent<Emoji> EmojiCreated { get => Event<GatewayEvent<Emoji>>(); set => Keep(value); }

    public delegate void GuildEmojiDeletedEvent(Emoji emoji);

    public GatewayEvent<Emoji> EmojiDeleted { get => Event<GatewayEvent<Emoji>>(); set => Keep(value); }

    public delegate void GuildStickerCreatedEvent(Sticker sticker);

    public GatewayEvent<Sticker> StickerCreated { get => Event<GatewayEvent<Sticker>>(); set => Keep(value); }

    public delegate void GuildStickerDeletedEvent(Sticker sticker);

    public GatewayEvent<Sticker> StickerDeleted { get => Event<GatewayEvent<Sticker>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_EMOJIS_UPDATE events when guild emojis are updated.
    /// </summary>
    public delegate void GuildEmojiUpdatedEvent(Emoji before, Emoji after);

    /// <summary>
    /// Occurs when the list of emojis in a guild is updated (added, removed, or modified).
    /// </summary>
    public GatewayEvent<Emoji, Emoji> EmojiUpdated { get => Event<GatewayEvent<Emoji, Emoji>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for GUILD_STICKERS_UPDATE events when guild stickers are updated.
    /// </summary>
    public delegate void GuildStickerUpdatedEvent(Sticker before, Sticker after);

    /// <summary>
    /// Occurs when the list of stickers in a guild is updated (added, removed, or modified).
    /// </summary>
    public GatewayEvent<Sticker, Sticker> StickerUpdated { get => Event<GatewayEvent<Sticker, Sticker>>(); set => Keep(value); }

    // ============================================================================
    // Relationship Events
    // ============================================================================

    /// <summary>
    /// Delegate for RELATIONSHIP_ADD events when a relationship is added.
    /// </summary>
    public delegate void RelationshipAddedEvent(SocketRelationship relationship);

    /// <summary>
    /// Occurs when a relationship (friend, blocked user, etc.) is added.
    /// </summary>
    public GatewayEvent<SocketRelationship> RelationshipAdded { get => Event<GatewayEvent<SocketRelationship>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for RELATIONSHIP_UPDATE events when a relationship is updated.
    /// </summary>
    public delegate void RelationshipUpdatedEvent(SocketRelationship relationship);

    /// <summary>
    /// Occurs when a relationship is updated.
    /// </summary>
    public GatewayEvent<SocketRelationship> RelationshipUpdated { get => Event<GatewayEvent<SocketRelationship>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for RELATIONSHIP_REMOVE events when a relationship is removed.
    /// </summary>
    public delegate void RelationshipRemovedEvent(ulong userId);

    /// <summary>
    /// Occurs when a relationship is removed.
    /// </summary>
    public GatewayEvent<ulong> RelationshipRemoved { get => Event<GatewayEvent<ulong>>(); set => Keep(value); }

    // ============================================================================
    // Favorite Meme Events
    // ============================================================================

    /// <summary>
    /// Delegate for FAVORITE_MEME_CREATE events when a favorite meme is created.
    /// </summary>
    public delegate void FavoriteMediaCreatedEvent(FavoriteMedia media);

    /// <summary>
    /// Occurs when a favorite meme is created.
    /// </summary>
    public GatewayEvent<FavoriteMedia> FavoriteMediaCreated { get => Event<GatewayEvent<FavoriteMedia>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for FAVORITE_MEME_UPDATE events when a favorite meme is updated.
    /// </summary>
    public delegate void FavoriteMediaUpdatedEvent(FavoriteMedia media);

    /// <summary>
    /// Occurs when a favorite meme is updated.
    /// </summary>
    public GatewayEvent<FavoriteMedia> FavoriteMediaUpdated { get => Event<GatewayEvent<FavoriteMedia>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for FAVORITE_MEME_DELETE events when a favorite meme is deleted.
    /// </summary>
    public delegate void FavoriteMediaDeletedEvent(string mediaId);

    /// <summary>
    /// Occurs when a favorite meme is deleted.
    /// </summary>
    public GatewayEvent<string> FavoriteMediaDeleted { get => Event<GatewayEvent<string>>(); set => Keep(value); }

    // ============================================================================
    // Call Events
    // ============================================================================

    /// <summary>
    /// Delegate for CALL_CREATE events when a call is created.
    /// </summary>
    public delegate void CallCreatedEvent(SocketCall call);

    /// <summary>
    /// Occurs when a call is created.
    /// </summary>
    public GatewayEvent<SocketCall> CallCreated { get => Event<GatewayEvent<SocketCall>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CALL_UPDATE events when a call is updated.
    /// </summary>
    public delegate void CallUpdatedEvent(SocketCall call);

    /// <summary>
    /// Occurs when a call is updated.
    /// </summary>
    public GatewayEvent<SocketCall> CallUpdated { get => Event<GatewayEvent<SocketCall>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for CALL_DELETE events when a call is deleted.
    /// </summary>
    public delegate void CallDeletedEvent(Channel channel, bool isUnavailable);

    /// <summary>
    /// Occurs when a call is deleted.
    /// </summary>
    public GatewayEvent<Channel, bool> CallDeleted { get => Event<GatewayEvent<Channel, bool>>(); set => Keep(value); }

    // ============================================================================
    // Invite Events
    // ============================================================================

    /// <summary>
    /// Delegate for INVITE_CREATE events when an invite is created.
    /// </summary>
    public delegate void InviteCreatedEvent(Invite invite);

    /// <summary>
    /// Occurs when a new invite is created for a guild or group DM.
    /// </summary>
    public GatewayEvent<Invite> InviteCreated { get => Event<GatewayEvent<Invite>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for INVITE_DELETE events when an invite is deleted or expires.
    /// </summary>
    public delegate void InviteDeletedEvent(Channel channel, SocketGuild? guild, string code);

    /// <summary>
    /// Occurs when an invite is deleted or expires.
    /// </summary>
    public GatewayEvent<Channel, SocketGuild?, string> InviteDeleted { get => Event<GatewayEvent<Channel, SocketGuild?, string>>(); set => Keep(value); }

    /// <summary>
    /// Delegate for INVITE_DELETE events that also hands over the deleted invite.
    /// </summary>
    /// <param name="channel">The channel the invite was for.</param>
    /// <param name="guild">The guild the invite was for, if any.</param>
    /// <param name="code">The invite code.</param>
    /// <param name="invite">The cached invite, or null when it wasn't cached.</param>
    public delegate void InviteRemovedEvent(Channel channel, SocketGuild? guild, string code, Invite? invite);

    /// <summary>
    /// Occurs when an invite is deleted, after <see cref="InviteDeleted"/>. Turn on <see cref="FluxerCacheConfig.Invites"/>
    /// to get the deleted invite.
    /// </summary>
    public GatewayEvent<Channel, SocketGuild?, string, Invite?> InviteRemoved { get => Event<GatewayEvent<Channel, SocketGuild?, string, Invite?>>(); set => Keep(value); }

    public delegate void GuildMemberCountsEvent(GuildMemberCountGatewayData[] guilds);

    public GatewayEvent<GuildMemberCountGatewayData[]> GuildMemberCounts { get => Event<GatewayEvent<GuildMemberCountGatewayData[]>>(); set => Keep(value); }

    public delegate void ChannelMemberCountsEvent(GuildChannelMemberCountGatewayData[] channels);

    public GatewayEvent<GuildChannelMemberCountGatewayData[]> ChannelMemberCounts { get => Event<GatewayEvent<GuildChannelMemberCountGatewayData[]>>(); set => Keep(value); }

    #endregion

    #endregion

    #region Voice

    /// <summary>
    /// Updates the current user's voice state (join/leave voice channels, mute, deafen).
    /// </summary>
    /// <remarks>
    /// This sends a VOICE_STATE_UPDATE packet to the gateway. The server will respond with
    /// VOICE_STATE_UPDATE and VOICE_SERVER_UPDATE events containing connection information.
    /// Without a connection id a channel opens a new connection, and leaving a guild needs the id of the
    /// connection to drop.
    /// </remarks>
    public void UpdateVoiceState(ulong? guildId, ulong? channelId, bool selfMute, bool selfDeaf, string? connectionId = null)
    {
        GatewayPacket packet = new GatewayPacket()
        {
            // TODO: Technically should be ulong, but fluxer expects strings. Fluxer will eventually be lenient and accept both.
            Data = JToken.FromObject(new VoiceStateUpdatePayload(guildId?.ToString(), channelId?.ToString(), selfMute, selfDeaf, connectionId)),
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

            _sendQueue.Dispose();
            StopEventDispatch();

            try
            {
                if (!Cache.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(5)))
                    _logger.Warning("Cache stores took longer than 5 seconds to dispose");
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error disposing cache stores");
            }

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
