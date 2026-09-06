namespace Fluxer.Net;

public class SocketGuildChannel : SocketChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketGuildChannel(FluxerBaseClient client) : base(client)
    {

    }
}
