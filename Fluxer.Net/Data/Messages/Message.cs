using Newtonsoft.Json;

namespace Fluxer.Net;

public class Message : Entity
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("bucket")]
    public int Bucket { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("author_id")]
    public ulong? AuthorId { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("webhook_id")]
    public ulong? WebhookId { get; set; }

    [JsonProperty("webhook_name")]
    public string? WebhookName { get; set; }

    [JsonProperty("webhook_avatar_hash")]
    public string? WebhookAvatarHash { get; set; }

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("edited_timestamp")]
    public DateTime? EditedTimestamp { get; set; }

    [JsonProperty("pinned_timestamp")]
    public DateTime? PinnedTimestamp { get; set; }

    [JsonProperty("flags")]
    public int Flags { get; set; }

    [JsonProperty("mention_everyone")]
    public bool MentionEveryone { get; set; }

    [JsonProperty("mention_users")]
    public HashSet<ulong>? MentionedUserIds { get; set; }

    [JsonProperty("mention_roles")]
    public HashSet<ulong>? MentionedRoleIds { get; set; }

    [JsonProperty("mention_channels")]
    public HashSet<ulong>? MentionedChannelIds { get; set; }

    [JsonProperty("attachments")]
    public List<Attachment>? Attachments { get; set; }

    [JsonProperty("embeds")]
    public List<Embed>? Embeds { get; set; }

    [JsonProperty("sticker_items")]
    public List<StickerItem>? Stickers { get; set; }

    [JsonProperty("message_reference")]
    public MessageRef? Reference { get; set; }

    [JsonProperty("message_snapshots")]
    public List<MessageSnapshot>? MessageSnapshots { get; set; }

    [JsonProperty("call")]
    public CallInfo? Call { get; set; }
}
