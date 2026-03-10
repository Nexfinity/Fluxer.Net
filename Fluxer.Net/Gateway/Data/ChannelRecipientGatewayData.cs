using Fluxer.Net.Data.Users;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for CHANNEL_RECIPIENT_ADD and CHANNEL_RECIPIENT_REMOVE events.
/// </summary>
public class ChannelRecipientGatewayData : IGatewayData
{
	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("user")]
	public User User { get; set; } = null!;
}
