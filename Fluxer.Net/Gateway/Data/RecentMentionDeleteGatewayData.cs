using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for RECENT_MENTION_DELETE event when a recent mention is deleted.
/// </summary>
public class RecentMentionDeleteGatewayData : IGatewayData
{
	[JsonProperty("message_id")]
	public ulong MessageId { get; set; }
}
