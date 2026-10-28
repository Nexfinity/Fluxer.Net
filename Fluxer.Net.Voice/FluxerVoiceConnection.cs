using LiveKit.Rtc;
using TrackSource = LiveKit.Proto.TrackSource;

namespace Fluxer.Net.Voice;

/// <summary>
/// The bot's connection to one voice channel, used to send audio into it.
/// </summary>
/// <remarks>
/// Audio is 16 bit signed PCM at <see cref="SampleRate"/> Hz with <see cref="Channels"/> interleaved channels.
/// Frames of 10 or 20 milliseconds work best.
/// </remarks>
public sealed class FluxerVoiceConnection : IAudioClient
{
    /// <inheritdoc cref="IAudioClient.SampleRate" />
    public const int SampleRate = IAudioClient.SampleRate;

    /// <inheritdoc cref="IAudioClient.Channels" />
    public const int Channels = IAudioClient.Channels;

    private readonly FluxerVoiceClient _owner;
    private readonly SemaphoreSlim _connectLock = new SemaphoreSlim(1, 1);
    private Room? _room;
    private AudioSource? _source;
    private bool _disposed;

    internal FluxerVoiceConnection(FluxerVoiceClient owner, ulong guildId, ulong channelId)
    {
        _owner = owner;
        GuildId = guildId;
        ChannelId = channelId;
    }

    /// <summary>
    /// The guild of the voice channel.
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    /// The voice channel the bot is connected to.
    /// </summary>
    public ulong ChannelId { get; internal set; }

    /// <summary>
    /// Fluxer's id for this voice connection, or null before the first voice server arrives.
    /// </summary>
    public string? ConnectionId { get; private set; }

    /// <summary>
    /// Whether the connection is open and audio can be sent.
    /// </summary>
    public bool IsConnected => _room?.IsConnected == true && _source != null;

    /// <summary>
    /// How much audio is queued and not yet sent.
    /// </summary>
    public TimeSpan QueuedDuration => _source == null ? TimeSpan.Zero : TimeSpan.FromMilliseconds(_source.QueuedDuration);

    /// <summary>
    /// Raised when the voice connection is lost or closed. The exception is null when it closed normally.
    /// </summary>
    public event Action<IAudioClient, Exception?>? Disconnected;

    /// <inheritdoc />
    public Stream CreatePcmStream() => new PcmStream(this);

    /// <summary>
    /// Sends one frame of audio, waiting when the queue is full.
    /// </summary>
    /// <param name="samples">Interleaved 16 bit samples for <see cref="Channels"/> channels at <see cref="SampleRate"/> Hz.</param>
    /// <param name="cancellationToken">Cancels the wait for room in the queue.</param>
    public Task SendAsync(short[] samples, CancellationToken cancellationToken = default)
    {
        AudioSource source = _source ?? throw new InvalidOperationException("The voice connection is not open.");
        if (samples.Length % Channels != 0)
            throw new ArgumentException($"The sample count must be a multiple of {Channels}.", nameof(samples));

        return source.CaptureFrameAsync(new AudioFrame(samples, SampleRate, Channels, samples.Length / Channels), cancellationToken);
    }

    /// <summary>
    /// Drops queued audio that has not been sent yet, for example when skipping or stopping playback.
    /// </summary>
    public void ClearQueue() => _source?.ClearQueue();

    /// <summary>
    /// Leaves the voice channel.
    /// </summary>
    public Task DisconnectAsync() => _owner.LeaveAsync(GuildId);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        _disposed = true;
        await _connectLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await CloseRoomAsync().ConfigureAwait(false);
        }
        finally
        {
            _connectLock.Release();
        }
    }

    internal async Task ConnectAsync(VoiceServer server, CancellationToken cancellationToken)
    {
        await _connectLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            RoomOptions options = new RoomOptions { AutoSubscribe = false };
            if (!string.IsNullOrEmpty(server.E2EEKey))
            {
                options.E2EE = new E2EEOptions
                {
                    KeyProviderOptions = new KeyProviderOptions { SharedKey = System.Text.Encoding.UTF8.GetBytes(server.E2EEKey) }
                };
            }

            string url = server.Endpoint.StartsWith("ws", StringComparison.OrdinalIgnoreCase)
                ? server.Endpoint
                : $"wss://{server.Endpoint}";

            Room room = new Room();
            room.Disconnected += (_, _) => OnRoomDisconnected(room);
            AudioSource source = new AudioSource(SampleRate, Channels, (int)Math.Clamp(_owner.QueueDuration.TotalMilliseconds, 20, 10000));
            try
            {
                await room.ConnectAsync(url, server.Token, options).ConfigureAwait(false);
                LocalAudioTrack track = LocalAudioTrack.Create("audio", source);
                await room.LocalParticipant!.PublishTrackAsync(track, new TrackPublishOptions
                {
                    Source = TrackSource.SourceMicrophone
                }, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await CloseAsync(room, source).ConfigureAwait(false);
                throw;
            }

            Room? previousRoom = _room;
            AudioSource? previousSource = _source;
            _room = room;
            _source = source;
            ChannelId = server.ChannelId;
            ConnectionId = server.ConnectionId;
            await CloseAsync(previousRoom, previousSource).ConfigureAwait(false);
        }
        finally
        {
            _connectLock.Release();
        }
    }

    internal void RaiseDisconnected(Exception? exception) => Disconnected?.Invoke(this, exception);

    private void OnRoomDisconnected(Room room)
    {
        if (!ReferenceEquals(room, _room) || _disposed)
            return;
        RaiseDisconnected(null);
    }

    private Task CloseRoomAsync()
    {
        Room? room = _room;
        _room = null;
        AudioSource? source = _source;
        _source = null;
        return CloseAsync(room, source);
    }

    private static async Task CloseAsync(Room? room, AudioSource? source)
    {
        if (room != null)
        {
            try
            {
                await room.DisconnectAsync().ConfigureAwait(false);
            }
            catch
            {
            }
            room.Dispose();
        }

        source?.Dispose();
    }
}
