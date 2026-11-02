using System.Collections.Concurrent;
using Fluxer.Net.Gateway;

namespace Fluxer.Net.Voice;

/// <summary>
/// Joins and leaves voice channels for a <see cref="FluxerClient"/> and keeps its <see cref="FluxerVoiceConnection"/>s.
/// </summary>
/// <remarks>
/// Fluxer voice runs on LiveKit. Connections reconnect on their own when Fluxer moves the bot to another voice server.
/// A bot can be in several voice channels of one guild: <see cref="JoinAsync"/> keeps one connection per guild and
/// moves it, and <see cref="JoinAdditionalAsync"/> opens another one. Creating a voice client sets it as the client's
/// <see cref="FluxerBaseClient.Voice"/>, so <see cref="IVoiceChannel.ConnectAsync"/> joins through it.
/// </remarks>
public sealed class FluxerVoiceClient : IVoiceProvider, IDisposable
{
    private readonly FluxerClient _client;
    private readonly object _gate = new object();
    private readonly ConcurrentDictionary<ulong, FluxerVoiceConnection> _connections = new ConcurrentDictionary<ulong, FluxerVoiceConnection>();
    private readonly ConcurrentDictionary<ulong, FluxerVoiceConnection[]> _guildConnections = new ConcurrentDictionary<ulong, FluxerVoiceConnection[]>();
    private readonly ConcurrentDictionary<ulong, PendingJoin> _pendingJoins = new ConcurrentDictionary<ulong, PendingJoin>();
    private readonly ConcurrentDictionary<ulong, ConnectionRecord> _connectionIds = new ConcurrentDictionary<ulong, ConnectionRecord>();
    private readonly ConcurrentDictionary<FluxerVoiceConnection, CancellationTokenSource> _pendingCloses = new ConcurrentDictionary<FluxerVoiceConnection, CancellationTokenSource>();
    private bool _disposed;

    /// <summary>
    /// Creates a voice client for a Fluxer client.
    /// </summary>
    public FluxerVoiceClient(FluxerClient client)
    {
        _client = client;
        _client.Voice = this;
        _client.Gateway.VoiceServerUpdated += OnVoiceServerUpdated;
        _client.Gateway.VoiceStateUpdated += OnVoiceStateUpdated;
        _client.Gateway.GatewayErrorReceived += OnGatewayError;
    }

    /// <summary>
    /// How long to wait for Fluxer to answer a join with a voice server.
    /// </summary>
    public TimeSpan JoinTimeout { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// How much audio a connection buffers before <see cref="FluxerVoiceConnection.SendAsync"/> waits.
    /// </summary>
    /// <remarks>
    /// Applies to connections opened after it is changed.
    /// </remarks>
    public TimeSpan QueueDuration { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// How long to wait for a new voice server after Fluxer reports the bot left voice, before treating it as a
    /// disconnect instead of a move.
    /// </summary>
    public TimeSpan MoveTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The highest bitrate audio is sent at, in bits per second, when the voice channel has none set.
    /// </summary>
    /// <remarks>
    /// Connections otherwise use the voice channel's bitrate as the limit. Applies to connections opened after it is
    /// changed.
    /// </remarks>
    public int DefaultBitrate { get; set; } = 128000;

    /// <summary>
    /// The open voice connections by guild id, with the first joined connection of each guild.
    /// </summary>
    public IReadOnlyDictionary<ulong, FluxerVoiceConnection> Connections =>
        _guildConnections.ToDictionary(pair => pair.Key, pair => pair.Value[0]);

    /// <summary>
    /// The open voice connections, by voice channel id.
    /// </summary>
    public IReadOnlyDictionary<ulong, FluxerVoiceConnection> ChannelConnections => _connections;

    /// <summary>
    /// Gets a voice connection in a guild, or null when the bot is not in voice there. When the bot is in several
    /// channels of the guild, the one joined first is returned.
    /// </summary>
    public FluxerVoiceConnection? GetConnection(ulong guildId) =>
        _guildConnections.TryGetValue(guildId, out FluxerVoiceConnection[]? connections) ? connections[0] : null;

    /// <summary>
    /// Gets the connection to a voice channel, or null when the bot is not in it.
    /// </summary>
    public FluxerVoiceConnection? GetConnection(ulong guildId, ulong channelId) =>
        _connections.TryGetValue(channelId, out FluxerVoiceConnection? connection) && connection.GuildId == guildId ? connection : null;

    /// <summary>
    /// Gets every voice connection the bot has in a guild.
    /// </summary>
    public IReadOnlyList<FluxerVoiceConnection> GetConnections(ulong guildId) =>
        _guildConnections.TryGetValue(guildId, out FluxerVoiceConnection[]? connections) ? connections : [];

    /// <summary>
    /// Joins a voice channel, or moves to it when the bot is already in voice in that guild. When the bot is already
    /// connected to that channel the open connection is returned, and a dead connection to it is replaced.
    /// </summary>
    /// <param name="guildId">The guild of the voice channel.</param>
    /// <param name="channelId">The voice channel to join.</param>
    /// <param name="selfDeaf">Whether the bot joins deafened. Bots that only send audio should leave this on.</param>
    /// <param name="selfMute">Whether the bot joins muted.</param>
    /// <param name="cancellationToken">Cancels the join.</param>
    /// <returns>The open connection.</returns>
    /// <exception cref="VoiceJoinTimeoutException">Fluxer did not send a voice server in time.</exception>
    /// <exception cref="VoiceJoinRefusedException">Fluxer refused the join, for example because the channel is full.</exception>
    public Task<FluxerVoiceConnection> JoinAsync(ulong guildId, ulong channelId, bool selfDeaf = true,
        bool selfMute = false, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        FluxerVoiceConnection? current = GetConnection(guildId, channelId);
        if (current != null && IsLive(current))
            return Task.FromResult(current);

        return JoinCoreAsync(guildId, channelId, current ?? GetConnection(guildId), selfDeaf, selfMute, cancellationToken);
    }

    /// <summary>
    /// Joins a voice channel as another connection, leaving the bot's other connections in that guild open. When the
    /// bot is already connected to that channel the open connection is returned, and a dead connection to it is
    /// replaced.
    /// </summary>
    /// <param name="guildId">The guild of the voice channel.</param>
    /// <param name="channelId">The voice channel to join.</param>
    /// <param name="selfDeaf">Whether the bot joins deafened. Bots that only send audio should leave this on.</param>
    /// <param name="selfMute">Whether the bot joins muted.</param>
    /// <param name="cancellationToken">Cancels the join.</param>
    /// <returns>The open connection.</returns>
    /// <exception cref="VoiceJoinTimeoutException">Fluxer did not send a voice server in time.</exception>
    /// <exception cref="VoiceJoinRefusedException">Fluxer refused the join, for example because the channel is full.</exception>
    public Task<FluxerVoiceConnection> JoinAdditionalAsync(ulong guildId, ulong channelId, bool selfDeaf = true,
        bool selfMute = false, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        FluxerVoiceConnection? current = GetConnection(guildId, channelId);
        if (current != null && IsLive(current))
            return Task.FromResult(current);

        return JoinCoreAsync(guildId, channelId, current, selfDeaf, selfMute, cancellationToken);
    }

    async Task<IAudioClient> IVoiceProvider.ConnectAsync(ulong guildId, ulong channelId, bool selfDeaf, bool selfMute,
        CancellationToken cancellationToken) =>
        await JoinAsync(guildId, channelId, selfDeaf, selfMute, cancellationToken).ConfigureAwait(false);

    async Task<IAudioClient> IVoiceProvider.ConnectAdditionalAsync(ulong guildId, ulong channelId, bool selfDeaf,
        bool selfMute, CancellationToken cancellationToken) =>
        await JoinAdditionalAsync(guildId, channelId, selfDeaf, selfMute, cancellationToken).ConfigureAwait(false);

    Task IVoiceProvider.DisconnectAsync(ulong guildId) => LeaveAsync(guildId);

    Task IVoiceProvider.DisconnectAsync(ulong guildId, ulong channelId) => LeaveAsync(guildId, channelId);

    IAudioClient? IVoiceProvider.GetAudioClient(ulong guildId) => GetConnection(guildId);

    IAudioClient? IVoiceProvider.GetAudioClient(ulong guildId, ulong channelId) => GetConnection(guildId, channelId);

    IReadOnlyCollection<IAudioClient> IVoiceProvider.GetAudioClients(ulong guildId) => GetConnections(guildId);

    /// <summary>
    /// Leaves voice in a guild and closes every connection the bot has there.
    /// </summary>
    public async Task LeaveAsync(ulong guildId)
    {
        foreach (KeyValuePair<ulong, ConnectionRecord> record in _connectionIds)
        {
            if (record.Value.GuildId == guildId && FindByConnectionId(record.Value.ConnectionId) == null)
                _client.Gateway.UpdateVoiceState(guildId, null, false, false, record.Value.ConnectionId);
        }

        foreach (FluxerVoiceConnection connection in GetConnections(guildId))
            await LeaveAsync(connection).ConfigureAwait(false);
    }

    /// <summary>
    /// Leaves one voice channel and closes its connection, keeping the bot's other connections in that guild.
    /// </summary>
    public Task LeaveAsync(ulong guildId, ulong channelId)
    {
        if (GetConnection(guildId, channelId) is { } connection)
            return LeaveAsync(connection);

        if (_connectionIds.TryGetValue(channelId, out ConnectionRecord record) && record.GuildId == guildId)
            _client.Gateway.UpdateVoiceState(guildId, null, false, false, record.ConnectionId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (ReferenceEquals(_client.Voice, this))
            _client.Voice = null;
        _client.Gateway.VoiceServerUpdated -= OnVoiceServerUpdated;
        _client.Gateway.VoiceStateUpdated -= OnVoiceStateUpdated;
        _client.Gateway.GatewayErrorReceived -= OnGatewayError;

        foreach (FluxerVoiceConnection connection in _pendingCloses.Keys)
            CancelClose(connection);
        foreach (FluxerVoiceConnection connection in _connections.Values)
            _ = connection.DisposeAsync().AsTask();
        lock (_gate)
        {
            _connections.Clear();
            _guildConnections.Clear();
        }
    }

    internal async Task LeaveAsync(FluxerVoiceConnection connection)
    {
        CancelClose(connection);
        if ((connection.ConnectionId ?? StrayConnectionId(connection.ChannelId)) is { } connectionId)
            _client.Gateway.UpdateVoiceState(connection.GuildId, null, false, false, connectionId);

        if (Untrack(connection))
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            connection.RaiseDisconnected(null);
        }
    }

    private async Task<FluxerVoiceConnection> JoinCoreAsync(ulong guildId, ulong channelId, FluxerVoiceConnection? existing,
        bool selfDeaf, bool selfMute, CancellationToken cancellationToken)
    {
        PendingJoin pending = new PendingJoin(guildId, existing);
        _pendingJoins[channelId] = pending;
        if (existing != null)
            CancelClose(existing);
        try
        {
            if (existing != null && existing.ChannelId == channelId && !existing.IsConnected)
            {
                await DropDeadConnectionAsync(existing).ConfigureAwait(false);
                existing = null;
                pending.Connection = null;
            }

            string? connectionId = existing != null
                ? existing.ConnectionId ?? StrayConnectionId(existing.ChannelId)
                : StrayConnectionId(channelId);
            _client.Gateway.UpdateVoiceState(guildId, channelId, selfMute, selfDeaf, connectionId);

            VoiceServer server;
            try
            {
                server = await pending.Source.Task.WaitAsync(JoinTimeout, cancellationToken).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                if (existing == null && StrayConnectionId(channelId) is { } strayId)
                    _client.Gateway.UpdateVoiceState(guildId, null, false, false, strayId);
                throw new VoiceJoinTimeoutException(guildId);
            }

            FluxerVoiceConnection connection = existing ?? Track(new FluxerVoiceConnection(this, guildId, channelId));
            try
            {
                await connection.ConnectAsync(server, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                Untrack(connection);
                _client.Gateway.UpdateVoiceState(guildId, null, false, false, server.ConnectionId);
                await connection.DisposeAsync().ConfigureAwait(false);
                throw;
            }

            Move(connection, server.ChannelId);
            return connection;
        }
        finally
        {
            _pendingJoins.TryRemove(new KeyValuePair<ulong, PendingJoin>(channelId, pending));
        }
    }

    private void OnVoiceServerUpdated(VoiceServer server)
    {
        if (server.GuildId is null)
            return;

        if (_pendingJoins.TryGetValue(server.ChannelId, out PendingJoin? pending) && pending.Source.TrySetResult(server))
            return;

        FluxerVoiceConnection? connection = FindByConnectionId(server.ConnectionId)
            ?? _connections.GetValueOrDefault(server.ChannelId);
        if (connection != null)
        {
            CancelClose(connection);
            _ = ReconnectAsync(connection, server);
        }
    }

    private void OnGatewayError(GatewayErrorGatewayData error)
    {
        if (_pendingJoins.Count != 1)
            return;

        foreach (PendingJoin pending in _pendingJoins.Values)
            pending.Source.TrySetException(new VoiceJoinRefusedException(pending.GuildId, error.Code, error.Message));
    }

    private void OnVoiceStateUpdated(SocketVoiceState state)
    {
        if (state.GuildId is not { } guildId || _client.Gateway.CurrentUser is not { } currentUser || state.UserId != currentUser.Id)
            return;

        if (state.ChannelId is { } channelId)
        {
            if (state.ConnectionId == null)
                return;

            ForgetConnectionId(state.ConnectionId, channelId);
            _connectionIds[channelId] = new ConnectionRecord(guildId, state.ConnectionId);
            if (FindByConnectionId(state.ConnectionId) is { } moved)
                Move(moved, channelId);
            return;
        }

        if (state.ConnectionId != null)
            ForgetConnectionId(state.ConnectionId, null);

        IEnumerable<FluxerVoiceConnection> closing = state.ConnectionId != null
            ? FindByConnectionId(state.ConnectionId) is { } match ? [match] : []
            : GetConnections(guildId);

        foreach (FluxerVoiceConnection connection in closing)
        {
            if (_pendingJoins.Values.Any(pending => ReferenceEquals(pending.Connection, connection)))
                continue;

            CancellationTokenSource close = new CancellationTokenSource();
            _pendingCloses.AddOrUpdate(connection, close, (_, previous) =>
            {
                previous.Cancel();
                return close;
            });
            _ = CloseAfterMoveTimeoutAsync(connection, close);
        }
    }

    private async Task CloseAfterMoveTimeoutAsync(FluxerVoiceConnection connection, CancellationTokenSource close)
    {
        try
        {
            await Task.Delay(MoveTimeout, close.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            _pendingCloses.TryRemove(new KeyValuePair<FluxerVoiceConnection, CancellationTokenSource>(connection, close));
        }

        if (Untrack(connection))
            await CloseAsync(connection).ConfigureAwait(false);
    }

    private async Task DropDeadConnectionAsync(FluxerVoiceConnection connection)
    {
        Untrack(connection);
        if ((connection.ConnectionId ?? StrayConnectionId(connection.ChannelId)) is { } connectionId)
            _client.Gateway.UpdateVoiceState(connection.GuildId, null, false, false, connectionId);

        _connectionIds.TryRemove(connection.ChannelId, out _);
        await connection.DisposeAsync().ConfigureAwait(false);
    }

    private void CancelClose(FluxerVoiceConnection connection)
    {
        if (_pendingCloses.TryRemove(connection, out CancellationTokenSource? close))
            close.Cancel();
    }

    internal uint GetBitrate(ulong channelId)
    {
        int bitrate = _client.Gateway.GetChannel(channelId)?.Bitrate is > 0 and var channelBitrate ? channelBitrate : DefaultBitrate;
        if (bitrate <= 512)
            bitrate *= 1000;
        return (uint)Math.Clamp(bitrate, 8000, 510000);
    }

    private bool IsLive(FluxerVoiceConnection connection) =>
        connection.IsConnected && !_pendingCloses.ContainsKey(connection);

    private string? StrayConnectionId(ulong channelId) =>
        _connectionIds.TryGetValue(channelId, out ConnectionRecord record) ? record.ConnectionId : null;

    private void ForgetConnectionId(string connectionId, ulong? keepChannelId)
    {
        foreach (KeyValuePair<ulong, ConnectionRecord> record in _connectionIds)
        {
            if (record.Value.ConnectionId == connectionId && record.Key != keepChannelId)
                _connectionIds.TryRemove(record);
        }
    }

    private FluxerVoiceConnection? FindByConnectionId(string connectionId) =>
        _connections.Values.FirstOrDefault(connection => connection.ConnectionId == connectionId);

    private FluxerVoiceConnection Track(FluxerVoiceConnection connection)
    {
        lock (_gate)
        {
            if (_connections.TryGetValue(connection.ChannelId, out FluxerVoiceConnection? tracked))
                return tracked;

            _connections[connection.ChannelId] = connection;
            _guildConnections.AddOrUpdate(connection.GuildId, [connection], (_, connections) => [.. connections, connection]);
            return connection;
        }
    }

    private bool Untrack(FluxerVoiceConnection connection)
    {
        lock (_gate)
        {
            if (!_connections.TryRemove(new KeyValuePair<ulong, FluxerVoiceConnection>(connection.ChannelId, connection)))
                return false;

            if (_guildConnections.TryGetValue(connection.GuildId, out FluxerVoiceConnection[]? connections))
            {
                FluxerVoiceConnection[] remaining = connections.Where(other => !ReferenceEquals(other, connection)).ToArray();
                if (remaining.Length == 0)
                    _guildConnections.TryRemove(connection.GuildId, out _);
                else
                    _guildConnections[connection.GuildId] = remaining;
            }

            return true;
        }
    }

    private void Move(FluxerVoiceConnection connection, ulong channelId)
    {
        lock (_gate)
        {
            if (connection.ChannelId == channelId)
                return;

            bool tracked = _connections.TryRemove(new KeyValuePair<ulong, FluxerVoiceConnection>(connection.ChannelId, connection));
            connection.ChannelId = channelId;
            if (tracked)
                _connections[channelId] = connection;
        }
    }

    private async Task ReconnectAsync(FluxerVoiceConnection connection, VoiceServer server)
    {
        try
        {
            await connection.ConnectAsync(server, CancellationToken.None).ConfigureAwait(false);
            Move(connection, server.ChannelId);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            if (Untrack(connection))
                await connection.DisposeAsync().ConfigureAwait(false);
            connection.RaiseDisconnected(ex);
        }
    }

    private static async Task CloseAsync(FluxerVoiceConnection connection)
    {
        await connection.DisposeAsync().ConfigureAwait(false);
        connection.RaiseDisconnected(null);
    }

    private sealed class PendingJoin(ulong guildId, FluxerVoiceConnection? connection)
    {
        public ulong GuildId { get; } = guildId;

        public FluxerVoiceConnection? Connection { get; set; } = connection;

        public TaskCompletionSource<VoiceServer> Source { get; } = new TaskCompletionSource<VoiceServer>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private readonly record struct ConnectionRecord(ulong GuildId, string ConnectionId);
}
