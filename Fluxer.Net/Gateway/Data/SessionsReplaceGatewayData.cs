using Fluxer.Net.Data.Auth;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for SESSIONS_REPLACE event when auth sessions are replaced.
/// </summary>
public class SessionsReplaceGatewayData : IGatewayData
{
	[JsonProperty("sessions")]
	public List<AuthSession> Sessions { get; set; } = new();
}
