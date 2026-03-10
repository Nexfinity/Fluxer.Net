using Fluxer.Net.Data.Voice;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for CALL_CREATE, CALL_UPDATE, and CALL_DELETE events.
/// </summary>
public class CallGatewayData : IGatewayData
{
	[JsonProperty("call")]
	public CallInfo Call { get; set; } = null!;
}
