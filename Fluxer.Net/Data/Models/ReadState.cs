using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class ReadState
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("message_id")]
	public ulong? LastMessageId { get; set; }

	[JsonProperty("mention_count")]
	public int MentionCount { get; set; }

	[JsonProperty("last_pin_timestamp")]
	public DateTime? LastPinTimestamp { get; set; }
}
