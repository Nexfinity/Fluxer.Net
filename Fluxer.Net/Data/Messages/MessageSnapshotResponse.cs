using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageSnapshotResponse
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
    public EmbedJson[]? Embeds { get; set; }

    [JsonProperty("attachments")]
    public MessageAttachmentJson[]? Attachments { get; set; }

    [JsonProperty("stickers")]
    public MessageStickerJson[]? Stickers { get; set; }

    [JsonRequired]
    [JsonProperty("type")]
    public MessageType Type { get; set; }

    [JsonRequired]
    [JsonProperty("flags")]
    public MessageFlags Flags { get; set; }
}
