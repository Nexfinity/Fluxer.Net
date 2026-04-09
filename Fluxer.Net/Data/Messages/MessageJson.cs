using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class MessageJson : IMessage
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("author")]
    public UserJson Author { get; set; }

    /// <inheritdoc />
    [JsonProperty("webhook_id")]
    public ulong? WebhookId { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public MessageType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public MessageFlag Flags { get; set; }

    /// <inheritdoc />
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <inheritdoc />
    [JsonProperty("timestamp")]
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("edited_timestamp")]
    public DateTime? EditedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("pinned")]
    public bool IsPinned { get; set; }

    /// <inheritdoc />
    [JsonProperty("mention_everyone")]
    public bool MentionEveryone { get; set; }

    /// <inheritdoc />
    [JsonProperty("tts")]
    public bool IsTts { get; set; }

    /// <inheritdoc />
    [JsonProperty("mentions")]
    public UserJson[]? Mentions { get; set; }

    /// <inheritdoc />
    [JsonProperty("mention_roles")]
    public ulong[]? MentionRoles { get; set; }

    /// <inheritdoc />
    [JsonProperty("embeds")]
    public List<EmbedJson>? Embeds { get; set; }

    /// <inheritdoc />
    [JsonProperty("attachments")]
    public AttachmentJson[]? Attachments { get; set; }

    /// <inheritdoc />
    [JsonProperty("stickers")]
    public StickerJson[]? Stickers { get; set; }

    /// <inheritdoc />
    [JsonProperty("reactions")]
    public MessageReactionJson[]? Reactions { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_reference")]
    public MessageReferenceJson? MessageReference { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_snapshots")]
    public MessageSnapshotJson[]? MessageSnapshots { get; set; }

    /// <inheritdoc />
    [JsonProperty("nonce")]
    public string? Nonce { get; set; }

    /// <inheritdoc />
    [JsonProperty("call")]
    public MessageCallJson? Call { get; set; }

    IUser IMessage.Author => Author;

    IEnumerable<IUser>? IMessage.Mentions => Mentions;

    ISticker[]? IMessage.Stickers => Stickers;

    IEnumerable<IEmbed>? IMessage.Embeds => Embeds;

    IAttachment[]? IMessage.Attachments => Attachments;

    IMessageReaction[]? IMessage.Reactions => Reactions;

    IMessageReference? IMessage.MessageReference => MessageReference;

    IMessageSnapshot[]? IMessage.MessageSnapshots => MessageSnapshots;

    IMessageCall? IMessage.Call => Call;
}
