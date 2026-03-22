using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class HelloGatewayData
{
    [JsonProperty("heartbeat_interval")]
    public int HeartbeatInterval { get; set; }

}
