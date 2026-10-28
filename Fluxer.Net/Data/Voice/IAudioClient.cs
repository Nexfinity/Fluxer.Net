namespace Fluxer.Net;

/// <summary>
/// The bot's connection to one voice channel, used to send audio into it.
/// </summary>
/// <remarks>
/// Audio is 16 bit signed PCM at <see cref="SampleRate"/> Hz with <see cref="Channels"/> interleaved channels.
/// </remarks>
public interface IAudioClient : IAsyncDisposable
{
    /// <summary>
    /// The sample rate audio must be sent at.
    /// </summary>
    const int SampleRate = 48000;

    /// <summary>
    /// The number of interleaved channels audio must be sent with.
    /// </summary>
    const int Channels = 2;

    /// <summary>
    /// The guild of the voice channel.
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    /// The voice channel the bot is connected to.
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    /// Whether the connection is open and audio can be sent.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// How much audio is queued and not yet sent.
    /// </summary>
    TimeSpan QueuedDuration { get; }

    /// <summary>
    /// Raised when the voice connection is lost or closed. The exception is null when it closed normally.
    /// </summary>
    event Action<IAudioClient, Exception?>? Disconnected;

    /// <summary>
    /// Creates a stream that sends the 16 bit PCM bytes written to it, in frames of 20 milliseconds.
    /// </summary>
    Stream CreatePcmStream();

    /// <summary>
    /// Sends one frame of audio, waiting when the queue is full.
    /// </summary>
    /// <param name="samples">Interleaved 16 bit samples for <see cref="Channels"/> channels at <see cref="SampleRate"/> Hz.</param>
    /// <param name="cancellationToken">Cancels the wait for room in the queue.</param>
    Task SendAsync(short[] samples, CancellationToken cancellationToken = default);

    /// <summary>
    /// Drops queued audio that has not been sent yet.
    /// </summary>
    void ClearQueue();

    /// <summary>
    /// Leaves the voice channel.
    /// </summary>
    Task DisconnectAsync();
}
