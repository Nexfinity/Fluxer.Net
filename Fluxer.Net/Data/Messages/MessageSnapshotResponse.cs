using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageSnapshotResponse : Entity
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonRequired]
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("edited_timestamp")]
    public DateTime? EditedTimestamp { get; set; }

    [JsonProperty("mentions")]
    public HashSet<ulong>? MentionedUserIds { get; set; }

    [JsonProperty("mention_roles")]
    public HashSet<ulong>? MentionedRoleIds { get; set; }

    [JsonProperty("embeds")]
    public MessageEmbedResponse[]? Embeds { get; set; }

    [JsonProperty("attachments")]
    public MessageAttachmentResponse[]? Attachments { get; set; }

    [JsonProperty("stickers")]
    public MessageStickerResponse[]? Stickers { get; set; }

    [JsonRequired]
    [JsonProperty("type")]
    public MessageType Type { get; set; }

    [JsonRequired]
    [JsonProperty("flags")]
    public MessageFlags Flags { get; set; }
}
