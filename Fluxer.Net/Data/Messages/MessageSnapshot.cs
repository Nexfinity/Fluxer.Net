namespace Fluxer.Net;

/// <inheritdoc />
public class MessageSnapshot : Entity, IMessageSnapshot
{
    /// <inheritdoc />
    public string? Content { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? EditedAt { get; private set; }

    /// <inheritdoc />
    public HashSet<ulong>? MentionedUserIds { get; private set; }

    /// <inheritdoc />
    public HashSet<ulong>? MentionedRoleIds { get; private set; }

    /// <inheritdoc />
    public Embed[]? Embeds { get; private set; }

    /// <inheritdoc />
    public Attachment[]? Attachments { get; private set; }

    /// <inheritdoc />
    public Sticker[]? Stickers { get; private set; }

    /// <inheritdoc />
    public MessageType Type { get; private set; }

    /// <inheritdoc />
    public MessageFlag Flags { get; private set; }

    IEmbed[]? IMessageSnapshot.Embeds => Embeds;

    IAttachment[]? IMessageSnapshot.Attachments => Attachments;

    ISticker[]? IMessageSnapshot.Stickers => Stickers;

    internal ulong ChannelId { get; set; }

    internal MessageSnapshot(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a MessageSnapshot object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static MessageSnapshot Create(FluxerBaseClient client, MessageSnapshotJson json, ulong channelId)
    {
        MessageSnapshot data = new MessageSnapshot(client)
        {
            ChannelId = channelId
        };
        data.Update(json);
        return data;
    }

    internal void Update(MessageSnapshotJson json)
    {
        Content = json.Content;
        CreatedAt = json.CreatedAt;
        EditedAt = json.EditedAt;
        MentionedUserIds = json.MentionedUserIds;
        MentionedRoleIds = json.MentionedRoleIds;

        if (json.Embeds != null)
            Embeds = json.Embeds.Select(x => Embed.Create(Client, x)).ToArray();

        if (json.Attachments != null)
            Attachments = json.Attachments.Select(x => Attachment.Create(Client, x, ChannelId)).ToArray();

        if (json.Stickers != null)
            Stickers = json.Stickers.Select(x => Sticker.Create(Client, x)).ToArray();

        Type = json.Type;
        Flags = json.Flags;
    }
}