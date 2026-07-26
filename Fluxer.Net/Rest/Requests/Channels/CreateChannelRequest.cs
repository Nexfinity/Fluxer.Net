using Newtonsoft.Json;

namespace Fluxer.Net;

public abstract class CreateGuildChannelRequest
{
    [JsonProperty("type")]
    public abstract string Type { get; }

    [JsonProperty("topic")]
    public string? Topic { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentCategoryId { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    [JsonProperty("voice_connection_limit")]
    public int? VoiceConnectionLimit { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? RatelimitPerUser { get; set; }

    [JsonProperty("permission_overwrites")]
    public ChannelOverwriteRequest[]? PermissionOverwrites { get; set; }
}
