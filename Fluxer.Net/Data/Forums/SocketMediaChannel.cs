namespace Fluxer.Net;

public class SocketMediaChannel : SocketForumChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketMediaChannel(FluxerBaseClient client) : base(client)
    {

    }

}