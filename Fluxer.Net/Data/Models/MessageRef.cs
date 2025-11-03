using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class MessageRef
{
	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("message_id")]
	public ulong MessageId { get; set; }

	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }
}
