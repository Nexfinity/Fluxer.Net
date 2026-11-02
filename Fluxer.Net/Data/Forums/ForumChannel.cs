namespace Fluxer.Net;

/// <summary>
/// A forum channel made of posts.
/// </summary>
public class ForumChannel : Channel, IForumChannel
{
    /// <inheritdoc />
    public ulong? CategoryId => ParentId;

    internal ForumChannel(FluxerBaseClient client) : base(client)
    {

    }
}