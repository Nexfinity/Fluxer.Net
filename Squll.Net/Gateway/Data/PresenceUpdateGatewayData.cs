using Newtonsoft.Json;

namespace Squll.Net.Gateway.Data;

public class PresenceUpdateGatewayData : IGatewayData
{
    [JsonProperty("status")]
    public string Status { get; set; }

    // [JsonProperty("activities")]
    // public object[] Activities { get; set; } = Array.Empty<object>();

    public PresenceUpdateGatewayData(string status)
        => Status = status;
}
