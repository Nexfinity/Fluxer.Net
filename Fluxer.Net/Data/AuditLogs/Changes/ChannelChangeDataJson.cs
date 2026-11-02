using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the channel.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The type of the channel.
    /// </summary>
    [JsonProperty("type")]
    public ChannelType Type { get; set; }

    /// <summary>
    /// The name of the channel.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The topic of the channel.
    /// </summary>
    [JsonProperty("topic")]
    public string? Topic { get; set; }

    /// <summary>
    /// The id of the parent category.
    /// </summary>
    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    /// <summary>
    /// The position of the channel in the channel list.
    /// </summary>
    [JsonProperty("position")]
    public int? Position { get; set; }

    /// <summary>
    /// Whether the channel is marked NSFW.
    /// </summary>
    [JsonProperty("nsfw")]
    public bool? IsNsfw { get; set; }

    /// <summary>
    /// The content warning level of the channel.
    /// </summary>
    [JsonProperty("content_warning_level")]
    public GuildContentWarning? ContentWarningLevel { get; set; }

    /// <summary>
    /// The content warning text of the channel.
    /// </summary>
    [JsonProperty("content_warning_text")]
    public string? ContentWarningText { get; set; }

    /// <summary>
    /// The slowmode delay in seconds between messages from one user.
    /// </summary>
    [JsonProperty("rate_limit_per_user")]
    public int? RateLimitPerUser { get; set; }

    /// <summary>
    /// The maximum number of users in a voice channel.
    /// </summary>
    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    /// <summary>
    /// The bitrate of a voice channel.
    /// </summary>
    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    /// <summary>
    /// The voice region of the channel.
    /// </summary>
    [JsonProperty("rtc_region")]
    public string? RtcRegion { get; set; }
}
