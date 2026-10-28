using System.Collections.Concurrent;

namespace Fluxer.Net;

public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel
{
    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, SocketVoiceState>();

    internal SocketVoiceChannel(FluxerBaseClient client) : base(client)
    {

    }
}
