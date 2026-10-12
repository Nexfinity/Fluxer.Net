using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ChannelGatewayData : ChannelJson
{
    [JsonProperty("recipients")]
    public List<UserJson>? Recipients { get; set; }
}