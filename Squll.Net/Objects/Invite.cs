using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class Invite
{
	[JsonProperty("code")]
	public string Code { get; set; }

	[JsonProperty("squad_id")]
	public ulong SquadID { get; set; }

	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("inviter")]
	public User Inviter { get; set; }

	[JsonProperty("uses")]
	public int Uses { get; set; }

	[JsonProperty("max_uses")]
	public int MaxUses { get; set; }

	[JsonProperty("max_age")]
	public int MaxAge { get; set; }

	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonProperty("expires_at")]
	public DateTime ExpiresAt { get; set; }
}
