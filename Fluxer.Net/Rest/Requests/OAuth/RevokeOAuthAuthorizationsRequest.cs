using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Revokes your OAuth2 authorizations of several applications.
/// </summary>
public class RevokeOAuthAuthorizationsRequest
{
    /// <summary>
    /// The applications to revoke, between 1 and 100.
    /// </summary>
    [JsonProperty("application_ids")]
    public IEnumerable<ulong> ApplicationIds { get; set; }
}
