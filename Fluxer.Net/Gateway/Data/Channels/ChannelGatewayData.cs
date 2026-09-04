using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway channel data matching the ChannelResponse API model
/// </summary>
public class ChannelGatewayData : ChannelJson
{
    [JsonProperty("recipients")]
    public List<UserJson>? Recipients { get; set; }
}