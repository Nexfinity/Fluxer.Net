namespace Fluxer.Net;

public class SocketLinkChannel : LinkChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketLinkChannel(FluxerBaseClient client) : base(client)
    {

    }

}
