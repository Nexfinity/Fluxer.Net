using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class GuildRole
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("permissions")]
	public ulong Permissions { get; set; }

	[JsonProperty("position")]
	public int Position { get; set; }

	[JsonProperty("color")]
	public int Color { get; set; }

	[JsonProperty("icon")]
	public string? IconHash { get; set; }

	[JsonProperty("unicode_emoji")]
	public string? UnicodeEmoji { get; set; }

	[JsonProperty("hoist")]
	public bool IsHoisted { get; set; }

	[JsonProperty("mentionable")]
	public bool IsMentionable { get; set; }
}
