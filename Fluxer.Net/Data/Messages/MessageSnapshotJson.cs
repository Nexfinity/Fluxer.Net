using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageSnapshotJson
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("edited_timestamp")]
    public DateTime? EditedTimestamp { get; set; }

    [JsonProperty("mention_users")]
    public HashSet<ulong>? MentionedUserIds { get; set; }

    [JsonProperty("mention_roles")]
    public HashSet<ulong>? MentionedRoleIds { get; set; }

    [JsonProperty("mention_channels")]
    public HashSet<ulong>? MentionedChannelIds { get; set; }

    [JsonProperty("attachments")]
    public List<AttachmentJson>? Attachments { get; set; }

    [JsonProperty("embeds")]
    public List<EmbedJson>? Embeds { get; set; }

    [JsonProperty("sticker_items")]
    public List<StickerJson>? Stickers { get; set; }

    [JsonProperty("type")]
    public MessageType Type { get; set; }

    [JsonProperty("flags")]
    public MessageFlag Flags { get; set; }
}
