using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class Webhook
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("token")]
	public string Token { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }

	[JsonProperty("channel_id")]
	public ulong? ChannelId { get; set; }

	[JsonProperty("creator_id")]
	public ulong? CreatorId { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("avatar")]
	public string? AvatarHash { get; set; }
}
