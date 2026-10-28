namespace Fluxer.Net;

public class SocketMediaChannel : MediaChannel, ISocketGuildChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketMediaChannel(FluxerBaseClient client) : base(client)
    {

    }

}