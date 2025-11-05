using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for CHANNEL_PINS_ACK event when channel pins are acknowledged.
/// </summary>
public class ChannelPinsAckGatewayData : IGatewayData
{
	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("timestamp")]
	public DateTime? Timestamp { get; set; }
}
