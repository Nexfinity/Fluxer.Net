using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ChannelUpdateBulkGatewayData
{
    [JsonProperty("channels")]
    public List<ChannelJson> Channels { get; set; } = new();
}