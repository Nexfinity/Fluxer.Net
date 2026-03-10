using Fluxer.Net.Data.Users;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for invite events (INVITE_CREATE, INVITE_DELETE).
/// </summary>
public class InviteGatewayData : IGatewayData
{
	/// <summary>
	/// The invite code (unique identifier).
	/// </summary>
	[JsonProperty("code")]
	public string Code { get; set; }

	/// <summary>
	/// The guild ID this invite is for (if applicable).
	/// </summary>
	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }

	/// <summary>
	/// The channel ID this invite is for.
	/// </summary>
	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	/// <summary>
	/// The user who created the invite.
	/// </summary>
	[JsonProperty("inviter")]
	public User? Inviter { get; set; }

	/// <summary>
	/// The type of invite (Guild = 0, GroupDm = 1).
	/// </summary>
	[JsonProperty("type")]
	public int? Type { get; set; }

	/// <summary>
	/// When this invite was created.
	/// </summary>
	[JsonProperty("created_at")]
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// How many times this invite has been used.
	/// </summary>
	[JsonProperty("uses")]
	public int? Uses { get; set; }

	/// <summary>
	/// Max number of times this invite can be used (0 = unlimited).
	/// </summary>
	[JsonProperty("max_uses")]
	public int? MaxUses { get; set; }

	/// <summary>
	/// Duration (in seconds) after which the invite expires (0 = never).
	/// </summary>
	[JsonProperty("max_age")]
	public int? MaxAge { get; set; }

	/// <summary>
	/// Whether this invite grants temporary membership.
	/// </summary>
	[JsonProperty("temporary")]
	public bool? Temporary { get; set; }
}
