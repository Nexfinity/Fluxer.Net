using Newtonsoft.Json;

namespace Squll.Net.Gateway;

public class StatusUpdateGatewayData : IGatewayData
{
    [JsonProperty("status")]
    public string Status { get; set; }

    public StatusUpdateGatewayData(string status)
        => Status = status;
}
