namespace Fluxer.Net;

/// <summary>
/// A cached forum tag.
/// </summary>
public class SocketForumTag : ForumTag
{

    internal SocketForumTag(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a cached forum tag from JSON.
    /// </summary>
    public static SocketForumTag Create(FluxerBaseClient client, ForumTagJson json)
    {
        SocketForumTag data = new SocketForumTag(client);
        data.Update(json);
        return data;
    }
}
