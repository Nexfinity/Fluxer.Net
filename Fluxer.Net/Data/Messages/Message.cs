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
    public DateTime Timestamp { get; internal set; }

    /// <inheritdoc />
    public DateTime? EditedTimestamp { get; internal set; }

    /// <inheritdoc />
    public bool Pinned { get; internal set; }

    /// <inheritdoc />
    public bool MentionEveryone { get; internal set; }

    /// <inheritdoc />
    public bool Tts { get; internal set; }

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
    public MessageReactionResponse[]? Reactions { get; internal set; }

    /// <inheritdoc />
    public MessageReferenceResponse? MessageReference { get; internal set; }

    /// <inheritdoc />
    public MessageSnapshotResponse[]? MessageSnapshots { get; internal set; }

    /// <inheritdoc />
    public string? Nonce { get; internal set; }

    /// <inheritdoc />
    public MessageCallJson? Call { get; internal set; }

    IUser IMessage.Author => Author;

    IEnumerable<IUser>? IMessage.Mentions => Mentions;

    ISticker[]? IMessage.Stickers => Stickers;

    IEnumerable<IEmbed>? IMessage.Embeds => Embeds;

    IAttachment[]? IMessage.Attachments => Attachments;

    internal Message(FluxerBaseClient client) : base(client)
    {

    }

    public static Message Create(FluxerBaseClient client, MessageJson json)
    {
        var data = new Message(client);
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
        Timestamp = json.Timestamp;
        EditedTimestamp = json.EditedTimestamp;
        Pinned = json.Pinned;
        MentionEveryone = json.MentionEveryone;
        Tts = json.Tts;
        if (json.Mentions != null)
            Mentions = json.Mentions.Select(x => User.Create(client, x));

        MentionRoles = json.MentionRoles;
        if (json.Embeds != null)
            Embeds = json.Embeds.Select(x => Embed.Create(client, x));

        if (json.Attachments != null)
            Attachments = json.Attachments.Select(x => Attachment.Create(client, x)).ToArray();

        if (json.Stickers != null)
            Stickers = json.Stickers.Select(x => Sticker.Create(client, x)).ToArray();

        Reactions = json.Reactions;
        MessageReference = json.MessageReference;
        MessageSnapshots = json.MessageSnapshots;
        Nonce = json.Nonce;
        Call = json.Call;
    }
}
