using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public abstract class CreateThreadRequest : CreateGuildChannelRequest
{
    [JsonProperty("name")]
    public string? Title { get; set; }

    [JsonProperty("auto_archive_duration")]
    public int? ArchiveDuration { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? Slowmode { get; set; }
}