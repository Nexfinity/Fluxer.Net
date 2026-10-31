using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A data harvest that was requested.
/// </summary>
public class HarvestCreationJson
{
    /// <summary>
    /// The harvest id.
    /// </summary>
    [JsonProperty("harvest_id")]
    public ulong HarvestId { get; set; }

    /// <summary>
    /// "pending", "processing", "completed" or "failed".
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// When the harvest was requested.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// The result of ignoring incoming friend requests in bulk.
/// </summary>
public class BulkIgnoreFriendRequestsJson
{
    /// <summary>
    /// How many friend requests were ignored.
    /// </summary>
    [JsonProperty("ignored_count")]
    public int IgnoredCount { get; set; }
}
