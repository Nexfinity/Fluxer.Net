using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for AUTH_SESSION_CHANGE event when an auth session changes.
/// </summary>
public class AuthSessionChangeGatewayData
{
    [JsonProperty("session")]
    public AuthSessionJson Session { get; set; } = null!;
}
