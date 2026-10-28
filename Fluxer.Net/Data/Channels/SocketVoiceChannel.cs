using System.Collections.Concurrent;

namespace Fluxer.Net;

public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel
{
    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, SocketVoiceState>();

    internal SocketVoiceChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <inheritdoc />
    public Task<IAudioClient> ConnectAsync(bool selfDeaf = true, bool selfMute = false,
        CancellationToken cancellationToken = default) =>
        Client.RequireVoice().ConnectAsync(GuildId!.Value, Id, selfDeaf, selfMute, cancellationToken);

    /// <inheritdoc />
    public Task DisconnectAsync() => Client.RequireVoice().DisconnectAsync(GuildId!.Value);
}
