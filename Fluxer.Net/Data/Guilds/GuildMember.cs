using Newtonsoft.Json;

namespace Fluxer.Net.Data.Guilds;

public class GuildMember
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("joined_at")]
	public DateTime JoinedAt { get; set; }

	[JsonProperty("nick")]
	public string? Nickname { get; set; }

	[JsonProperty("avatar")]
	public string? AvatarHash { get; set; }

	[JsonProperty("banner")]
	public string? BannerHash { get; set; }

	[JsonProperty("bio")]
	public string? Bio { get; set; }

	[JsonProperty("pronouns")]
	public string? Pronouns { get; set; }

	[JsonProperty("accent_color")]
	public int? AccentColor { get; set; }

	[JsonProperty("join_source_type")]
	public int? JoinSourceType { get; set; }

	[JsonProperty("source_invite_code")]
	public string? SourceInviteCode { get; set; }

	[JsonProperty("inviter_id")]
	public ulong? InviterId { get; set; }

	[JsonProperty("deaf")]
	public bool IsDeaf { get; set; }

	[JsonProperty("mute")]
	public bool IsMute { get; set; }

	[JsonProperty("communication_disabled_until")]
	public DateTime? CommunicationDisabledUntil { get; set; }

	[JsonProperty("roles")]
	public HashSet<ulong>? RoleIds { get; set; }

	[JsonProperty("is_premium_sanitized")]
	public bool IsPremiumSanitized { get; set; }

	[JsonProperty("temporary")]
	public bool IsTemporary { get; set; }
}
