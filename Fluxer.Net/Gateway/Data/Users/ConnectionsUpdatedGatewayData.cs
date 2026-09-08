using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class ConnectionsUpdatedGatewayData
{
    [JsonProperty("connections")]
    public UserConnectionJson[] Connections { get; set; }
}
