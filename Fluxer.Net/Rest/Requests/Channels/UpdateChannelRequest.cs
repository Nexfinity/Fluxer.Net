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
}
