using Newtonsoft.Json;

namespace Fluxer.Net;

public class ChannelChangeDataJson : IAuditLogData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("type")]
    public ChannelType Type { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("topic")]
    public string? Topic { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonProperty("position")]
    public int? Position { get; set; }

    [JsonProperty("nsfw")]
    public bool? IsNsfw { get; set; }

    [JsonProperty("content_warning_level")]
    public GuildContentWarning? ContentWarningLevel { get; set; }

    [JsonProperty("content_warning_text")]
    public string? ContentWarningText { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? RateLimitPerUser { get; set; }

    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    [JsonProperty("rtc_region")]
    public string? RtcRegion { get; set; }
}
