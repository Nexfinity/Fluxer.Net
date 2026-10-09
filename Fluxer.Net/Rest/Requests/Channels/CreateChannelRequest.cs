using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public abstract class CreateGuildChannelRequest
{
    [JsonProperty("type")]
    public abstract GuildChannelType Type { get; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("topic")]
    public string? Topic { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentCategoryId { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? RatelimitPerUser { get; set; }

    [JsonProperty("permission_overwrites")]
    public ChannelOverwriteRequest[]? PermissionOverwrites { get; set; }
}
