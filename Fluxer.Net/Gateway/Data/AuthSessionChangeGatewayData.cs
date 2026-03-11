using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for AUTH_SESSION_CHANGE event when an auth session changes.
/// </summary>
public class AuthSessionChangeGatewayData : IGatewayData
{
	[JsonProperty("session")]
	public AuthSession Session { get; set; } = null!;
}
