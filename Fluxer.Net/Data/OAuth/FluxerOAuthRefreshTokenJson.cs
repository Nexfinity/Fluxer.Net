using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthRefreshTokenJson : IFluxerOAuthRefreshToken
{
    /// <inheritdoc />
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <inheritdoc />
    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    /// <inheritdoc />
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    /// <inheritdoc />
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <inheritdoc />
    [JsonProperty("scope")]
    public string Scope { get; set; }
}
