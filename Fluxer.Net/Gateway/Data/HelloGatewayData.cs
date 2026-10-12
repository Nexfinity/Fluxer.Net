using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class HelloGatewayData
{
    [JsonProperty("heartbeat_interval")]
    public int HeartbeatInterval { get; set; }
}