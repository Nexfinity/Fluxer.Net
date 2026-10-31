using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Instructions for verifying a connection that is being added.
/// </summary>
public class ConnectionVerificationJson
{
    /// <summary>
    /// The verification token to place in DNS or on the profile.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <summary>
    /// The type of connection being verified, "bluesky" or "domain".
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// The connection identifier, a handle or a domain.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Human-readable instructions for completing verification.
    /// </summary>
    [JsonProperty("instructions")]
    public string Instructions { get; set; }

    /// <summary>
    /// Signed token to send back when verifying.
    /// </summary>
    [JsonProperty("initiation_token")]
    public string InitiationToken { get; set; }
}
