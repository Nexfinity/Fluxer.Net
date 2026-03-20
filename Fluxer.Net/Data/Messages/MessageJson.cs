using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/message/MessageResponseSchemas.tsx#L129"/>
/// </remarks>
public class MessageJson : IMessage
{
    /// <inheritdoc />
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    [JsonRequired]
    [JsonProperty("author")]
    public UserPartialResponse Author { get; set; }

    /// <inheritdoc />
    [JsonProperty("webhook_id")]
    public ulong? WebhookId { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public MessageType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public MessageFlags Flags { get; set; }

    /// <inheritdoc />
    [JsonRequired]
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <inheritdoc />
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <inheritdoc />
    [JsonProperty("edited_timestamp")]
    public DateTime? EditedTimestamp { get; set; }

    /// <inheritdoc />
    [JsonProperty("pinned")]
    public bool Pinned { get; set; }

    /// <inheritdoc />
    [JsonProperty("mention_everyone")]
    public bool MentionEveryone { get; set; }

    /// <inheritdoc />
    [JsonProperty("tts")]
    public bool Tts { get; set; }

    /// <inheritdoc />
    [JsonProperty("mentions")]
    public UserPartialResponse[]? Mentions { get; set; }

    /// <inheritdoc />
    [JsonProperty("mention_roles")]
    public ulong[]? MentionRoles { get; set; }

    /// <inheritdoc />
    [JsonProperty("embeds")]
    public MessageEmbedJson[]? Embeds { get; set; }

    /// <inheritdoc />
    [JsonProperty("attachments")]
    public MessageAttachmentJson[]? Attachments { get; set; }

    /// <inheritdoc />
    [JsonProperty("stickers")]
    public MessageStickerJson[]? Stickers { get; set; }

    /// <inheritdoc />
    [JsonProperty("reactions")]
    public MessageReactionResponse[]? Reactions { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_reference")]
    public MessageReferenceResponse? MessageReference { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_snapshots")]
    public MessageSnapshotResponse[]? MessageSnapshots { get; set; }

    /// <inheritdoc />
    [JsonProperty("nonce")]
    public string? Nonce { get; set; }

    /// <inheritdoc />
    [JsonProperty("call")]
    public MessageCallJson? Call { get; set; }
}
