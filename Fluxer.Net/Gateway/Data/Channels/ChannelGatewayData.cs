using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Channels;

/// <summary>
/// Gateway channel data matching the ChannelResponse API model
/// </summary>
public class ChannelGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("topic")]
    public string? Topic { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    [JsonProperty("owner_id")]
    public ulong? OwnerId { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("position")]
    public int? Position { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    [JsonProperty("rtc_region")]
    public string? RtcRegion { get; set; }

    [JsonProperty("last_message_id")]
    public ulong? LastMessageId { get; set; }

    [JsonProperty("last_pin_timestamp")]
    public DateTime? LastPinTimestamp { get; set; }

    [JsonProperty("permission_overwrites")]
    public List<ChannelOverwriteResponse>? PermissionOverwrites { get; set; }

    [JsonProperty("recipients")]
    public List<UserJson>? Recipients { get; set; }

    [JsonProperty("nsfw")]
    public bool IsNsfw { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? RateLimitPerUser { get; set; }

    [JsonProperty("nicks")]
    public Dictionary<string, string>? Nicks { get; set; }
}

/// <summary>
/// Channel permission overwrite response
/// </summary>
public class ChannelOverwriteResponse
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("allow")]
    public string Allow { get; set; } = null!;

    [JsonProperty("deny")]
    public string Deny { get; set; } = null!;
}
