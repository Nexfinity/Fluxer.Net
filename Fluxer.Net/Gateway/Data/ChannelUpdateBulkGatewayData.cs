using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for CHANNEL_UPDATE_BULK event when multiple channels are updated.
/// </summary>
public class ChannelUpdateBulkGatewayData : IGatewayData
{
	[JsonProperty("channels")]
	public List<Channel> Channels { get; set; } = new();
}
