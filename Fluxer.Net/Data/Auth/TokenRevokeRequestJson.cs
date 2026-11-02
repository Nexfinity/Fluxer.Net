using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The request body for revoking a token.
/// </summary>
public class TokenRevokeRequestJson
{
    /// <summary>
    /// The token to revoke.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }
}
