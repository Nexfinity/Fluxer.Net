using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Channels;

/// <summary>
/// Gateway data for CHANNEL_UPDATE_BULK event when multiple channels are updated.
/// </summary>
public class ChannelUpdateBulkGatewayData
{
    [JsonProperty("channels")]
    public List<ChannelJson> Channels { get; set; } = new();
}
