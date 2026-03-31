using Newtonsoft.Json;
using System.ComponentModel;

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

    [DefaultValue(false)]
    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("permission_overwrites")]
    public ChannelOverwriteRequest[]? PermissionOverwrites { get; set; }
}
