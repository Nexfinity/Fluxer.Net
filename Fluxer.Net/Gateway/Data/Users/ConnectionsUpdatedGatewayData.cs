using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ConnectionsUpdatedGatewayData
{
    [JsonProperty("connections")]
    public UserConnectionJson[] Connections { get; set; }
}