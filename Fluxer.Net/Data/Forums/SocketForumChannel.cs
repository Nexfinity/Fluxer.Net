namespace Fluxer.Net;

public class SocketForumChannel : ForumChannel
{
    public SocketGuild Guild { get; internal set; }

    internal SocketForumChannel(FluxerBaseClient client) : base(client)
    {

    }

}
