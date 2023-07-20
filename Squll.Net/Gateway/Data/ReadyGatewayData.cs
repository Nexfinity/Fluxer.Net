using Newtonsoft.Json;

namespace Squll.Net.Gateway;

public class ReadyGatewayData : IGatewayData
{
    [JsonProperty("heartbeat_interval")]
    public int HeartbeatInterval { get; set; }

}
