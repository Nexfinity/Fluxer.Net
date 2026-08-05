using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class HelloGatewayData
{
    [JsonProperty("heartbeat_interval")]
    public int HeartbeatInterval { get; set; }

}
