using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class UpdateThreadRequest
{
    [JsonProperty("name")]
    public string? Title { get; set; }

    [JsonProperty("auto_archive_duration")]
    public int? ArchiveDuration { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    [JsonProperty("locked")]
    public bool? IsLocked { get; set; }

    [JsonProperty("archived")]
    public bool? IsArchived { get; set; }

    /// <summary>
    /// IDs of the tags applied to the forum or media post (max 5).
    /// </summary>
    [JsonProperty("applied_tags")]
    public List<ulong>? AppliedTags { get; set; }
}