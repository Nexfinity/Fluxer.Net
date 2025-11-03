using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class RecentMention
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("message_id")]
	public ulong MessageId { get; set; }

	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("is_everyone")]
	public bool IsEveryone { get; set; }

	[JsonProperty("is_role")]
	public bool IsRole { get; set; }
}
