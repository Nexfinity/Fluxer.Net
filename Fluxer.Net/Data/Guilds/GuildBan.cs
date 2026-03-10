using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildBan
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("moderator_id")]
	public ulong ModeratorId { get; set; }

	[JsonProperty("banned_at")]
	public DateTime BannedAt { get; set; }

	[JsonProperty("expires_at")]
	public DateTime? ExpiresAt { get; set; }

	[JsonProperty("reason")]
	public string? Reason { get; set; }

	[JsonProperty("ip")]
	public string? IpAddress { get; set; }
}
