using Newtonsoft.Json;

namespace Fluxer.Net;


/// <inheritdoc />
public class ChannelJson : PartialChannelJson, IChannel
{
    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("topic")]
    public string? Topic { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    /// <inheritdoc />
    [JsonProperty("position")]
    public int Position { get; set; }

    /// <inheritdoc />
    [JsonProperty("owner_id")]
    public ulong? OwnerId { get; set; }

    /// <inheritdoc />
    [JsonProperty("recipient_ids")]
    public HashSet<ulong>? RecipientIds { get; set; }

    /// <inheritdoc />
    [JsonProperty("nsfw")]
    public bool IsNsfw { get; set; }

    /// <inheritdoc />
    [JsonProperty("rate_limit_per_user")]
    public int? RateLimitPerUser { get; set; }

    /// <inheritdoc />
    [JsonProperty("thread_metadata")]
    public ThreadMetadataJson? ThreadMetadata { get; set; }

    /// <inheritdoc />
    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    /// <inheritdoc />
    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    /// <inheritdoc />
    [JsonProperty("voice_connection_limit")]
    public int? VoiceConnectionLimit { get; set; }

    /// <inheritdoc />
    [JsonProperty("rtc_region")]
    public string? RtcRegion { get; set; }

    /// <inheritdoc />
    [JsonProperty("last_message_id")]
    public ulong? LastMessageId { get; set; }

    /// <inheritdoc />
    [JsonProperty("last_pin_timestamp")]
    public DateTimeOffset? LastPinAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("permission_overwrites")]
    public List<PermissionOverwriteJson>? PermissionOverwrites { get; set; }

    /// <inheritdoc />
    [JsonProperty("nicks")]
    public Dictionary<string, string>? Nicknames { get; set; }

    /// <inheritdoc />
    [JsonProperty("soft_deleted")]
    public bool IsSoftDeleted { get; set; }

    /// <inheritdoc />
    [JsonProperty("indexed_at")]
    public DateTimeOffset? IndexedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_warning_level")]
    public GuildContentWarning? ContentWarningLevel { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_warning_text")]
    public string? ContentWarningText { get; set; }

    /// <inheritdoc />
    [JsonProperty("member")]
    public ThreadMemberJson? ThreadMember { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public ChannelFlags Flags { get; set; }

    /// <inheritdoc />
    [JsonProperty("available_tags")]
    public ForumTagJson[]? ForumTags { get; set; }

    /// <inheritdoc />
    [JsonProperty("applied_tags")]
    public ulong[]? AppliedTags { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_reaction_emoji")]
    public ForumReactionEmoji? DefaultReactionEmoji { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_sort_order")]
    public ForumSortOrder? DefaultSortOrder { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_forum_layout")]
    public ForumLayout? DefaultLayout { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_auto_archive_duration")]
    public ThreadArchiveDuration? DefaultAutoArchiveDuration { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_thread_rate_limit_per_user")]
    public int? DefaultRateLimitPerUser { get; set; }

    IEnumerable<IPermissionOverwrite>? IChannel.PermissionOverwrites => PermissionOverwrites;

    IThreadMetadata? IChannel.ThreadMetadata => ThreadMetadata;

    IThreadMember? IChannel.ThreadMember => ThreadMember;

    IForumTag[]? IChannel.ForumTags => ForumTags;
}
