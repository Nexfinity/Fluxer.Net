using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for CALL_CREATE, CALL_UPDATE, and CALL_DELETE events.
/// </summary>
public class CallGatewayData
{
    [JsonProperty("call")]
    public CallInfoJson Call { get; set; } = null!;
}
