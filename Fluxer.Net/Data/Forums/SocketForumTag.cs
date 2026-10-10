namespace Fluxer.Net;

public class SocketForumTag : ForumTag
{

    internal SocketForumTag(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ForumTag object from json.
    /// </summary>
    /// <returns><see cref="SocketForumTag"/></returns>
    public static SocketForumTag Create(FluxerBaseClient client, ForumTagJson json)
    {
        SocketForumTag data = new SocketForumTag(client);
        data.Update(json);
        return data;
    }
}
