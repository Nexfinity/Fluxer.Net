namespace Fluxer.Net;

/// <summary>
/// The author and first message of a forum or media post.
/// </summary>
public class ForumPostData : Entity
{
    /// <summary>
    /// The member who created the post, or null when they left the guild.
    /// </summary>
    public GuildMember? Owner { get; private set; }

    /// <summary>
    /// The first message of the post, or null when it was deleted.
    /// </summary>
    public Message? FirstMessage { get; private set; }

    internal ForumPostData(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ForumPostData object from json.
    /// </summary>
    /// <returns><see cref="ForumPostData"/></returns>
    public static ForumPostData Create(FluxerBaseClient client, ForumPostDataJson json)
    {
        ForumPostData data = new ForumPostData(client);
        data.Owner = json.Owner == null ? null : GuildMember.Create(client, json.Owner);
        data.FirstMessage = json.FirstMessage == null ? null : Message.Create(client, json.FirstMessage);
        return data;
    }
}
