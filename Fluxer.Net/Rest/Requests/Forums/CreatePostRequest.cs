using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreatePostRequest
{
    [JsonProperty("name")]
    public string Title { get; set; }

    [JsonProperty("auto_archive_duration")]
    public ThreadArchiveDuration? ArchiveDuration { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    [JsonProperty("message")]
    public MessageRequest Message { get; set; }

    [JsonProperty("applied_tags")]
    public ulong[]? Tags { get; set; }
}
