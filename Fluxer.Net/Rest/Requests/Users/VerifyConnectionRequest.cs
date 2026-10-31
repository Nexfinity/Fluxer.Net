using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Finishes adding an external connection once its ownership can be verified.
/// </summary>
public class VerifyConnectionRequest
{
    /// <summary>
    /// The initiation token from <see cref="ConnectionVerificationJson.InitiationToken"/>.
    /// </summary>
    [JsonProperty("initiation_token")]
    public string InitiationToken { get; set; }

    /// <summary>
    /// Bitfield controlling who can see the connection.
    /// </summary>
    [JsonProperty("visibility_flags")]
    public int? VisibilityFlags { get; set; }
}
