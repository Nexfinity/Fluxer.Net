using Newtonsoft.Json;

namespace Squll.Net.Gateway;

public class PresenceUpdateGatewayData : IGatewayData
{
    [JsonProperty("status")]
    public string Status { get; set; }

    public PresenceUpdateGatewayData(string status)
        => Status = status;
}
