namespace Fluxer.Net;

/// <inheritdoc />
public class Message : Entity, IMessage
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public ulong ChannelId { get; internal set; }

    /// <inheritdoc />
    public User Author { get; internal set; }

    /// <inheritdoc />
    public ulong? WebhookId { get; internal set; }

    /// <inheritdoc />
    public MessageType Type { get; internal set; }

    /// <inheritdoc />
    public MessageFlag Flags { get; internal set; }

    /// <inheritdoc />
    public string Content { get; internal set; }

    /// <inheritdoc />
    public DateTime CreatedAt { get; internal set; }

    /// <inheritdoc />
    public DateTime? EditedAt { get; internal set; }

    /// <inheritdoc />
    public bool IsPinned { get; internal set; }

    /// <inheritdoc />
    public bool MentionEveryone { get; internal set; }

    /// <inheritdoc />
    public bool IsTts { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<User>? Mentions { get; internal set; }

    /// <inheritdoc />
    public ulong[]? MentionRoles { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<Embed>? Embeds { get; internal set; }

    /// <inheritdoc />
    public Attachment[]? Attachments { get; internal set; }

    /// <inheritdoc />
    public Sticker[]? Stickers { get; internal set; }

    /// <inheritdoc />
    public MessageReaction[]? Reactions { get; internal set; }

    /// <inheritdoc />
    public MessageReference? MessageReference { get; internal set; }

    /// <inheritdoc />
    public MessageSnapshot[]? MessageSnapshots { get; internal set; }

    /// <inheritdoc />
    public string? Nonce { get; internal set; }

    /// <inheritdoc />
    public MessageCall? Call { get; internal set; }

    IUser IMessage.Author => Author;

    IEnumerable<IUser>? IMessage.Mentions => Mentions;

    ISticker[]? IMessage.Stickers => Stickers;

    IEnumerable<IEmbed>? IMessage.Embeds => Embeds;

    IAttachment[]? IMessage.Attachments => Attachments;

    IMessageReaction[]? IMessage.Reactions => Reactions;

    IMessageReference? IMessage.MessageReference => MessageReference;

    IMessageSnapshot[]? IMessage.MessageSnapshots => MessageSnapshots;

    IMessageCall? IMessage.Call => Call;

    internal Message(FluxerBaseClient client) : base(client)
    {

    }

    public static Message Create(FluxerBaseClient client, MessageJson json)
    {
        Message data = new Message(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, MessageJson json)
    {
        Id = json.Id;
        ChannelId = json.ChannelId;
        Author = User.Create(client, json.Author);
        WebhookId = json.WebhookId;
        Type = json.Type;
        Flags = json.Flags;
        Content = json.Content;
        CreatedAt = json.CreatedAt;
        EditedAt = json.EditedAt;
        IsPinned = json.IsPinned;
        MentionEveryone = json.MentionEveryone;
        IsTts = json.IsTts;
        if (json.Mentions != null)
            Mentions = json.Mentions.Select(x => User.Create(client, x));

        MentionRoles = json.MentionRoles;
        if (json.Embeds != null)
            Embeds = json.Embeds.Select(x => Embed.Create(client, x));

        if (json.Attachments != null)
            Attachments = json.Attachments.Select(x => Attachment.Create(client, x, ChannelId)).ToArray();

        if (json.Stickers != null)
            Stickers = json.Stickers.Select(x => Sticker.Create(client, x)).ToArray();

        if (json.Reactions != null)
            Reactions = json.Reactions.Select(x => MessageReaction.Create(client, x)).ToArray();

        if (json.MessageReference != null)
            MessageReference = MessageReference.Create(client, json.MessageReference);

        if (json.MessageSnapshots != null)
            MessageSnapshots = json.MessageSnapshots.Select(x => MessageSnapshot.Create(client, x, ChannelId)).ToArray();

        Nonce = json.Nonce;

        if (json.Call != null)
            Call = MessageCall.Create(client, json.Call);
    }
}
