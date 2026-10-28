using System.Collections.Concurrent;
using Fluxer.Net.Gateway;

namespace Fluxer.Net.Voice;

/// <summary>
/// Joins and leaves voice channels for a <see cref="FluxerClient"/> and keeps one <see cref="FluxerVoiceConnection"/>
/// per guild.
/// </summary>
/// <remarks>
/// Fluxer voice runs on LiveKit. Connections reconnect on their own when Fluxer moves the bot to another voice server.
/// Creating a voice client sets it as the client's <see cref="FluxerBaseClient.Voice"/>, so
/// <see cref="IVoiceChannel.ConnectAsync"/> joins through it.
/// </remarks>
public sealed class FluxerVoiceClient : IVoiceProvider, IDisposable
{
    private readonly FluxerClient _client;
    private readonly ConcurrentDictionary<ulong, FluxerVoiceConnection> _connections = new ConcurrentDictionary<ulong, FluxerVoiceConnection>();
    private readonly ConcurrentDictionary<ulong, TaskCompletionSource<VoiceServer>> _pendingJoins = new ConcurrentDictionary<ulong, TaskCompletionSource<VoiceServer>>();
    private readonly ConcurrentDictionary<ulong, string> _connectionIds = new ConcurrentDictionary<ulong, string>();
    private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _pendingCloses = new ConcurrentDictionary<ulong, CancellationTokenSource>();
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
    /// The open voice connections, by guild id.
    /// </summary>
    public IReadOnlyDictionary<ulong, FluxerVoiceConnection> Connections => _connections;

    /// <summary>
    /// Gets the voice connection for a guild, or null when the bot is not in voice there.
    /// </summary>
    public FluxerVoiceConnection? GetConnection(ulong guildId) => _connections.GetValueOrDefault(guildId);

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
    public async Task<FluxerVoiceConnection> JoinAsync(ulong guildId, ulong channelId, bool selfDeaf = true,
        bool selfMute = false, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FluxerVoiceClient));

        if (_connections.TryGetValue(guildId, out FluxerVoiceConnection? current) && current.ChannelId == channelId
            && current.IsConnected && !_pendingCloses.ContainsKey(guildId))
            return current;

        TaskCompletionSource<VoiceServer> pending = new TaskCompletionSource<VoiceServer>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingJoins[guildId] = pending;
        CancelClose(guildId);
        try
        {
            if (current != null && current.ChannelId == channelId && !current.IsConnected)
                await DropDeadConnectionAsync(guildId, current).ConfigureAwait(false);

            _client.Gateway.UpdateVoiceState(guildId, channelId, selfMute, selfDeaf, GetConnectionId(guildId));

            VoiceServer server;
            try
            {
                server = await pending.Task.WaitAsync(JoinTimeout, cancellationToken).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                if (!_connections.ContainsKey(guildId) && GetConnectionId(guildId) is { } connectionId)
                    _client.Gateway.UpdateVoiceState(guildId, null, false, false, connectionId);
                throw new VoiceJoinTimeoutException(guildId);
            }

            FluxerVoiceConnection connection = _connections.GetOrAdd(guildId, id => new FluxerVoiceConnection(this, id, channelId));
            try
            {
                await connection.ConnectAsync(server, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                _connections.TryRemove(new KeyValuePair<ulong, FluxerVoiceConnection>(guildId, connection));
                _client.Gateway.UpdateVoiceState(guildId, null, false, false, server.ConnectionId);
                await connection.DisposeAsync().ConfigureAwait(false);
                throw;
            }

            return connection;
        }
        finally
        {
            _pendingJoins.TryRemove(new KeyValuePair<ulong, TaskCompletionSource<VoiceServer>>(guildId, pending));
        }
    }

    async Task<IAudioClient> IVoiceProvider.ConnectAsync(ulong guildId, ulong channelId, bool selfDeaf, bool selfMute,
        CancellationToken cancellationToken) =>
        await JoinAsync(guildId, channelId, selfDeaf, selfMute, cancellationToken).ConfigureAwait(false);

    Task IVoiceProvider.DisconnectAsync(ulong guildId) => LeaveAsync(guildId);

    IAudioClient? IVoiceProvider.GetAudioClient(ulong guildId) => GetConnection(guildId);

    /// <summary>
    /// Leaves voice in a guild and closes its connection.
    /// </summary>
    public async Task LeaveAsync(ulong guildId)
    {
        CancelClose(guildId);
        if (GetConnectionId(guildId) is { } connectionId)
            _client.Gateway.UpdateVoiceState(guildId, null, false, false, connectionId);

        if (_connections.TryRemove(guildId, out FluxerVoiceConnection? connection))
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            connection.RaiseDisconnected(null);
        }
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

        foreach (ulong guildId in _pendingCloses.Keys)
            CancelClose(guildId);
        foreach (FluxerVoiceConnection connection in _connections.Values)
            _ = connection.DisposeAsync().AsTask();
        _connections.Clear();
    }

    private void OnVoiceServerUpdated(VoiceServer server)
    {
        if (server.GuildId is not { } guildId)
            return;

        if (_pendingJoins.TryGetValue(guildId, out TaskCompletionSource<VoiceServer>? pending) && pending.TrySetResult(server))
            return;

        if (_connections.TryGetValue(guildId, out FluxerVoiceConnection? connection))
        {
            CancelClose(guildId);
            _ = ReconnectAsync(connection, server);
        }
    }

    private void OnGatewayError(GatewayErrorGatewayData error)
    {
        if (_pendingJoins.Count != 1)
            return;

        foreach (KeyValuePair<ulong, TaskCompletionSource<VoiceServer>> pending in _pendingJoins)
            pending.Value.TrySetException(new VoiceJoinRefusedException(pending.Key, error.Code, error.Message));
    }

    private void OnVoiceStateUpdated(SocketVoiceState state)
    {
        if (state.GuildId is not { } guildId || _client.Gateway.CurrentUser is not { } currentUser || state.UserId != currentUser.Id)
            return;

        if (state.ChannelId is { } channelId)
        {
            if (state.ConnectionId != null)
                _connectionIds[guildId] = state.ConnectionId;
            if (_connections.TryGetValue(guildId, out FluxerVoiceConnection? moved))
                moved.ChannelId = channelId;
            return;
        }

        if (state.ConnectionId != null)
            _connectionIds.TryRemove(new KeyValuePair<ulong, string>(guildId, state.ConnectionId));

        if (_pendingJoins.ContainsKey(guildId) || !_connections.TryGetValue(guildId, out FluxerVoiceConnection? connection))
            return;

        if (state.ConnectionId != null && connection.ConnectionId != null && state.ConnectionId != connection.ConnectionId)
            return;

        CancellationTokenSource close = new CancellationTokenSource();
        _pendingCloses.AddOrUpdate(guildId, close, (_, previous) =>
        {
            previous.Cancel();
            return close;
        });
        _ = CloseAfterMoveTimeoutAsync(guildId, connection, close);
    }

    private async Task CloseAfterMoveTimeoutAsync(ulong guildId, FluxerVoiceConnection connection, CancellationTokenSource close)
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
            _pendingCloses.TryRemove(new KeyValuePair<ulong, CancellationTokenSource>(guildId, close));
        }

        if (_connections.TryRemove(new KeyValuePair<ulong, FluxerVoiceConnection>(guildId, connection)))
            await CloseAsync(connection).ConfigureAwait(false);
    }

    private async Task DropDeadConnectionAsync(ulong guildId, FluxerVoiceConnection connection)
    {
        _connections.TryRemove(new KeyValuePair<ulong, FluxerVoiceConnection>(guildId, connection));
        string? connectionId = connection.ConnectionId ?? _connectionIds.GetValueOrDefault(guildId);
        if (connectionId != null)
            _client.Gateway.UpdateVoiceState(guildId, null, false, false, connectionId);

        _connectionIds.TryRemove(guildId, out _);
        await connection.DisposeAsync().ConfigureAwait(false);
    }

    private void CancelClose(ulong guildId)
    {
        if (_pendingCloses.TryRemove(guildId, out CancellationTokenSource? close))
            close.Cancel();
    }

    private string? GetConnectionId(ulong guildId) =>
        _connections.GetValueOrDefault(guildId)?.ConnectionId ?? _connectionIds.GetValueOrDefault(guildId);

    private async Task ReconnectAsync(FluxerVoiceConnection connection, VoiceServer server)
    {
        try
        {
            await connection.ConnectAsync(server, CancellationToken.None).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            if (_connections.TryRemove(new KeyValuePair<ulong, FluxerVoiceConnection>(connection.GuildId, connection)))
                await connection.DisposeAsync().ConfigureAwait(false);
            connection.RaiseDisconnected(ex);
        }
    }

    private static async Task CloseAsync(FluxerVoiceConnection connection)
    {
        await connection.DisposeAsync().ConfigureAwait(false);
        connection.RaiseDisconnected(null);
    }
}
