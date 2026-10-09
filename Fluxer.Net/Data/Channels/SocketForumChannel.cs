namespace Fluxer.Net;

public class SocketForumChannel : SocketGuildChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketForumChannel(FluxerBaseClient client) : base(client)
    {

    }

}
