using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating a guild channel, sending only the properties that are set.
/// </summary>
public class UpdateChannelRequest
{
    [JsonProperty("name")]
    public Optional<string?> Name { get; set; }

    [JsonProperty("topic")]
    public Optional<string?> Topic { get; set; }

    [JsonProperty("url")]
    public Optional<string?> Url { get; set; }

    [JsonProperty("parent_id")]
    public Optional<ulong?> ParentId { get; set; }

    [JsonProperty("nsfw_override")]
    public Optional<bool?> IsNsfw { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public Optional<int?> RateLimitPerUser { get; set; }

    [JsonProperty("bitrate")]
    public Optional<int?> Bitrate { get; set; }

    [JsonProperty("user_limit")]
    public Optional<int?> UserLimit { get; set; }

    [JsonProperty("rtc_region")]
    public Optional<string?> RtcRegion { get; set; }

    [JsonProperty("permission_overwrites")]
    public Optional<IEnumerable<ChannelOverwriteRequest>> PermissionOverwrites { get; set; }

    /// <summary>
    /// Whether voice calls connect participants directly, without a voice server.
    /// </summary>
    [JsonProperty("rtc_p2p")]
    public Optional<bool> RtcP2P { get; set; }

    /// <summary>
    /// The most voice connections one user can have in the channel at once, 1 to 100, or null for no limit.
    /// </summary>
    [JsonProperty("voice_connection_limit")]
    public Optional<int?> VoiceConnectionLimit { get; set; }

    /// <summary>
    /// The channel flags.
    /// </summary>
    [JsonProperty("flags")]
    public Optional<ChannelFlags> Flags { get; set; }

    /// <summary>
    /// Whether to show a content warning before entering the channel, or inherit it from the category or guild.
    /// </summary>
    [JsonProperty("content_warning_level")]
    public Optional<GuildContentWarning> ContentWarningLevel { get; set; }

    /// <summary>
    /// The text shown in the channel's content warning, or null for the default text.
    /// </summary>
    [JsonProperty("content_warning_text")]
    public Optional<string?> ContentWarningText { get; set; }

    /// <summary>
    /// The full list of forum tags. Tags left out are removed.
    /// </summary>
    [JsonProperty("available_tags")]
    public Optional<IEnumerable<ForumTagUpdateRequest>> ForumTags { get; set; }

    /// <summary>
    /// The reaction shown on new forum posts, or null for none.
    /// </summary>
    [JsonProperty("default_reaction_emoji")]
    public Optional<ForumReactionEmoji?> DefaultReactionEmoji { get; set; }

    /// <summary>
    /// How forum posts are sorted by default.
    /// </summary>
    [JsonProperty("default_sort_order")]
    public Optional<ForumSortOrder?> DefaultSortOrder { get; set; }

    /// <summary>
    /// How forum posts are laid out by default.
    /// </summary>
    [JsonProperty("default_forum_layout")]
    public Optional<ForumLayout?> DefaultLayout { get; set; }

    /// <summary>
    /// How the forum's tag search combines several picked tags.
    /// </summary>
    [JsonProperty("default_tag_setting")]
    public Optional<ForumTagMatchSetting> DefaultTagSetting { get; set; }

    /// <summary>
    /// How long new threads stay active before they archive.
    /// </summary>
    [JsonProperty("default_auto_archive_duration")]
    public Optional<ThreadArchiveDuration?> DefaultAutoArchiveDuration { get; set; }

    /// <summary>
    /// The slowmode, in seconds, given to new threads.
    /// </summary>
    [JsonProperty("default_thread_rate_limit_per_user")]
    public Optional<int?> DefaultRateLimitPerUser { get; set; }
}
