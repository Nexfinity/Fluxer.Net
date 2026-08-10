namespace Fluxer.Net;

/// <inheritdoc />
public class MessageSnapshot : Entity, IMessageSnapshot
{
    /// <inheritdoc />
    public string? Content { get; internal set; }

    /// <inheritdoc />
    public DateTime CreatedAt { get; internal set; }

    /// <inheritdoc />
    public DateTime? EditedAt { get; internal set; }

    /// <inheritdoc />
    public HashSet<ulong>? MentionedUserIds { get; internal set; }

    /// <inheritdoc />
    public HashSet<ulong>? MentionedRoleIds { get; internal set; }

    /// <inheritdoc />
    public Embed[]? Embeds { get; internal set; }

    /// <inheritdoc />
    public Attachment[]? Attachments { get; internal set; }

    /// <inheritdoc />
    public Sticker[]? Stickers { get; internal set; }

    /// <inheritdoc />
    public MessageType Type { get; internal set; }

    /// <inheritdoc />
    public MessageFlag Flags { get; internal set; }

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
        var data = new MessageSnapshot(client)
        {
            ChannelId = channelId
        };
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, MessageSnapshotJson json)
    {
        Content = json.Content;
        CreatedAt = json.CreatedAt;
        EditedAt = json.EditedAt;
        MentionedUserIds = json.MentionedUserIds;
        MentionedRoleIds = json.MentionedRoleIds;

        if (json.Embeds != null)
            Embeds = json.Embeds.Select(x => Embed.Create(client, x)).ToArray();

        if (json.Attachments != null)
            Attachments = json.Attachments.Select(x => Attachment.Create(client, x, ChannelId)).ToArray();

        if (json.Stickers != null)
            Stickers = json.Stickers.Select(x => Sticker.Create(client, x)).ToArray();

        Type = json.Type;
        Flags = json.Flags;
    }
}