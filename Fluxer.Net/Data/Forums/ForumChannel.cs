namespace Fluxer.Net;

public class ForumChannel : Channel, IForumChannel
{
    public ulong? CategoryId => ParentId;

    internal ForumChannel(FluxerBaseClient client) : base(client)
    {

    }
}