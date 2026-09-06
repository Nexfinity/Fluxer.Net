using System.Collections.Concurrent;

namespace Fluxer.Net;

public class SocketVoiceChannel : SocketTextChannel
{
    public SocketGuild Guild { get; internal set; }

    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; } = new ConcurrentDictionary<string, SocketVoiceState>();

    internal SocketVoiceChannel(FluxerBaseClient client) : base(client)
    {

    }
}
