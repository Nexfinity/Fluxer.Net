using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class MessageSnapshotJson : IMessageSnapshot
{
    /// <inheritdoc />
    [JsonProperty("content")]
    public string? Content { get; set; }

    /// <inheritdoc />
    [JsonProperty("timestamp")]
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("edited_timestamp")]
    public DateTime? EditedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("mentions")]
    public HashSet<ulong>? MentionedUserIds { get; set; }

    /// <inheritdoc />
    [JsonProperty("mention_roles")]
    public HashSet<ulong>? MentionedRoleIds { get; set; }

    /// <inheritdoc />
    [JsonProperty("embeds")]
    public EmbedJson[]? Embeds { get; set; }

    /// <inheritdoc />
    [JsonProperty("attachments")]
    public AttachmentJson[]? Attachments { get; set; }

    /// <inheritdoc />
    [JsonProperty("stickers")]
    public StickerJson[]? Stickers { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public MessageType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public MessageFlag Flags { get; set; }

    IEmbed[]? IMessageSnapshot.Embeds => Embeds;

    IAttachment[]? IMessageSnapshot.Attachments => Attachments;

    ISticker[]? IMessageSnapshot.Stickers => Stickers;
}
