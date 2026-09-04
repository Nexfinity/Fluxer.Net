namespace Fluxer.Net;

/// <inheritdoc />
public class Message : Entity, IMessage
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public ulong ChannelId { get; private set; }

    /// <inheritdoc />
    public User Author { get; private set; }

    /// <inheritdoc />
    public ulong? WebhookId { get; private set; }

    /// <inheritdoc />
    public MessageType Type { get; private set; }

    /// <inheritdoc />
    public MessageFlag Flags { get; private set; }

    /// <inheritdoc />
    public string Content { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? EditedAt { get; private set; }

    /// <inheritdoc />
    public bool IsPinned { get; private set; }

    /// <inheritdoc />
    public bool MentionEveryone { get; private set; }

    /// <inheritdoc />
    public bool IsTTS { get; private set; }

    /// <inheritdoc />
    public IEnumerable<User>? Mentions { get; private set; }

    /// <inheritdoc />
    public ulong[]? MentionRoles { get; private set; }

    /// <inheritdoc />
    public IEnumerable<Embed>? Embeds { get; private set; }

    /// <inheritdoc />
    public Attachment[]? Attachments { get; private set; }

    /// <inheritdoc />
    public Sticker[]? Stickers { get; private set; }

    /// <inheritdoc />
    public MessageReaction[]? Reactions { get; private set; }

    /// <inheritdoc />
    public MessageReference? MessageReference { get; private set; }

    /// <inheritdoc />
    public MessageSnapshot[]? MessageSnapshots { get; private set; }

    /// <inheritdoc />
    public string? Nonce { get; private set; }

    /// <inheritdoc />
    public MessageCall? Call { get; private set; }

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

    /// <summary>
    /// Create a Message object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Message Create(FluxerBaseClient client, MessageJson json)
    {
        Message data = new Message(client);
        data.Update(json);
        return data;
    }

    internal void Update(MessageJson json)
    {
        Id = json.Id;
        ChannelId = json.ChannelId;
        Author = User.Create(Client, json.Author);
        WebhookId = json.WebhookId;
        Type = json.Type;
        Flags = json.Flags;
        Content = json.Content;
        CreatedAt = json.CreatedAt;
        EditedAt = json.EditedAt;
        IsPinned = json.IsPinned;
        MentionEveryone = json.MentionEveryone;
        IsTTS = json.IsTTS;
        if (json.Mentions != null)
            Mentions = json.Mentions.Select(x => User.Create(Client, x));

        MentionRoles = json.MentionRoles;
        if (json.Embeds != null)
            Embeds = json.Embeds.Select(x => Embed.Create(Client, x));

        if (json.Attachments != null)
            Attachments = json.Attachments.Select(x => Attachment.Create(Client, x, ChannelId)).ToArray();

        if (json.Stickers != null)
            Stickers = json.Stickers.Select(x => Sticker.Create(Client, x)).ToArray();

        if (json.Reactions != null)
            Reactions = json.Reactions.Select(x => MessageReaction.Create(Client, x)).ToArray();

        if (json.MessageReference != null)
            MessageReference = MessageReference.Create(Client, json.MessageReference);

        if (json.MessageSnapshots != null)
            MessageSnapshots = json.MessageSnapshots.Select(x => MessageSnapshot.Create(Client, x, ChannelId)).ToArray();

        Nonce = json.Nonce;

        if (json.Call != null)
            Call = MessageCall.Create(Client, json.Call);
    }
}
