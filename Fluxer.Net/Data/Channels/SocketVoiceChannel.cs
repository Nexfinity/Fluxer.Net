using System.Collections.Concurrent;

namespace Fluxer.Net;

/// <summary>
/// A cached guild voice or stage channel.
/// </summary>
public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel
{
    /// <summary>
    /// The voice states in this channel, by connection ID, or session ID when there is none.
    /// </summary>
    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, SocketVoiceState>();

    internal SocketVoiceChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <inheritdoc />
    public Task<IAudioClient> ConnectAsync(bool selfDeaf = true, bool selfMute = false,
        CancellationToken cancellationToken = default) =>
        Client.RequireVoice().ConnectAsync(GuildId!.Value, Id, selfDeaf, selfMute, cancellationToken);

    /// <inheritdoc />
    public Task<IAudioClient> ConnectAdditionalAsync(bool selfDeaf = true, bool selfMute = false,
        CancellationToken cancellationToken = default) =>
        Client.RequireVoice().ConnectAdditionalAsync(GuildId!.Value, Id, selfDeaf, selfMute, cancellationToken);

    /// <inheritdoc />
    public Task DisconnectAsync() => Client.RequireVoice().DisconnectAsync(GuildId!.Value);
}
