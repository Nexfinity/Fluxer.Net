namespace Fluxer.Net;

public class ForumTag : Entity, IForumTag
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public ulong? EmojiId { get; private set; }

    /// <inheritdoc />
    public string? EmojiName { get; private set; }

    /// <inheritdoc />
    public bool IsModerated { get; private set; }

    /// <inheritdoc />
    public ulong ChannelId { get; private set; }

    internal ForumTag(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ForumTag object from json.
    /// </summary>
    /// <returns><see cref="ForumTag"/></returns>
    public static ForumTag Create(FluxerBaseClient client, ForumTagJson json, ulong channelId)
    {
        ForumTag data = new ForumTag(client)
        {
            ChannelId = channelId,
        };
        data.Update(json);
        return data;
    }

    internal virtual void Update(ForumTagJson json)
    {
        Id = json.Id;
        Name = json.Name;
        EmojiId = json.EmojiId;
        EmojiName = json.EmojiName;
        IsModerated = json.IsModerated;
    }
}