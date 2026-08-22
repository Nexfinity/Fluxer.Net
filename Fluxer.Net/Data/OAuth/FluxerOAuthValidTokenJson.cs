using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthValidTokenJson : IFluxerOAuthValidToken
{
    /// <inheritdoc />
    [JsonProperty("active")]
    public bool IsActive { get; set; }

    /// <inheritdoc />
    [JsonProperty("scope")]
    public string? Scope { get; set; }

    /// <inheritdoc />
    [JsonProperty("client_id")]
    public ulong? ClientId { get; set; }

    /// <inheritdoc />
    [JsonProperty("token_type")]
    public string? TokenType { get; set; }

    /// <inheritdoc />
    [JsonProperty("exp")]
    public int? Exp { get; set; }

    /// <inheritdoc />
    [JsonProperty("iat")]
    public int? Iat { get; set; }

    /// <inheritdoc />
    [JsonProperty("Sub")]
    public ulong? Sub { get; set; }
}
