using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthTokenJson : IFluxerOAuthToken
{
    /// <inheritdoc />
    [JsonProperty("application")]
    public PartialApplicationJson Application { get; set; }

    /// <inheritdoc />
    [JsonProperty("scopes")]
    public string[] Scopes { get; set; }

    /// <inheritdoc />
    [JsonProperty("expires")]
    public DateTime ExpiresAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public FluxerOAuthUserJson User { get; set; }

    IPartialApplication IFluxerOAuthToken.Application => Application;

    IFluxerOAuthUser IFluxerOAuthToken.User => User;
}
