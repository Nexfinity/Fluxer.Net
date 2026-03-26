using Newtonsoft.Json;

namespace Fluxer.Net.OAuth;

/// <inheritdoc />
public class FluxerOAuthUserJson : UserJson, IFluxerOAuthUser
{
    /// <inheritdoc />
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <inheritdoc />
    [JsonProperty("verified")]
    public bool? IsVerified { get; set; }
}
