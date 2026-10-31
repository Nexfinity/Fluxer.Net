using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Ignores incoming friend requests in bulk.
/// </summary>
public class BulkIgnoreFriendRequestsRequest
{
    /// <summary>
    /// "all" or "new_accounts".
    /// </summary>
    [JsonProperty("filter")]
    public string? Filter { get; set; }

    /// <summary>
    /// With "new_accounts", the oldest account age in seconds that is still ignored.
    /// </summary>
    [JsonProperty("max_account_age_seconds")]
    public long? MaxAccountAgeSeconds { get; set; }
}
