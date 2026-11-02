using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating a thread.
/// </summary>
public class UpdateThreadRequest
{
    /// <summary>
    /// The new name of the thread.
    /// </summary>
    [JsonProperty("name")]
    public string? Title { get; set; }

    /// <summary>
    /// How long the thread can be inactive before it is archived, in minutes.
    /// </summary>
    [JsonProperty("auto_archive_duration")]
    public int? ArchiveDuration { get; set; }

    /// <summary>
    /// The new slowmode delay in seconds between messages from one user.
    /// </summary>
    [JsonProperty("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    /// <summary>
    /// Whether the thread is locked.
    /// </summary>
    [JsonProperty("locked")]
    public bool? IsLocked { get; set; }

    /// <summary>
    /// Whether the thread is archived.
    /// </summary>
    [JsonProperty("archived")]
    public bool? IsArchived { get; set; }

    /// <summary>
    /// IDs of the tags applied to the forum or media post (max 5).
    /// </summary>
    [JsonProperty("applied_tags")]
    public List<ulong>? AppliedTags { get; set; }
}