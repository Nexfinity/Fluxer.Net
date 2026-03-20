using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// The nested guild properties object sent inside a GUILD_CREATE payload.
/// </summary>
public class GuildCreateProperties
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; } = null!;

	[JsonProperty("icon")]
	public string? Icon { get; set; }

	[JsonProperty("banner")]
	public string? Banner { get; set; }

	[JsonProperty("splash")]
	public string? Splash { get; set; }

	[JsonProperty("vanity_url_code")]
	public string? VanityUrlCode { get; set; }

	[JsonProperty("owner_id")]
	public ulong OwnerId { get; set; }

	[JsonProperty("system_channel_id")]
	public ulong? SystemChannelId { get; set; }

	[JsonProperty("system_channel_flags")]
	public int SystemChannelFlags { get; set; }

	[JsonProperty("rules_channel_id")]
	public ulong? RulesChannelId { get; set; }

	[JsonProperty("afk_channel_id")]
	public ulong? AfkChannelId { get; set; }

	[JsonProperty("afk_timeout")]
	public int AfkTimeout { get; set; }

	[JsonProperty("features")]
	public List<string> Features { get; set; } = new();

	[JsonProperty("verification_level")]
	public int VerificationLevel { get; set; }

	[JsonProperty("mfa_level")]
	public int MfaLevel { get; set; }

	[JsonProperty("nsfw_level")]
	public int NsfwLevel { get; set; }

	[JsonProperty("explicit_content_filter")]
	public int ExplicitContentFilter { get; set; }

	[JsonProperty("default_message_notifications")]
	public int DefaultMessageNotifications { get; set; }

	[JsonProperty("disabled_operations")]
	public int DisabledOperations { get; set; }

	[JsonProperty("max_presences")]
	public int? MaxPresences { get; set; }
}

/// <summary>
/// Gateway data for GUILD_CREATE and GUILD_UPDATE events.
/// Contains the full guild state including channels, members, roles, and nested guild properties.
/// When <see cref="Unavailable"/> is true the guild is temporarily unavailable (outage)
/// and only <see cref="Id"/> and <see cref="Unavailable"/> will be populated.
/// </summary>
public class GuildGatewayData : IGatewayData
{
	/// <summary>
	/// The guild ID.
	/// </summary>
	[JsonProperty("id")]
	public ulong Id { get; set; }

	/// <summary>
	/// When the current user joined this guild.
	/// </summary>
	[JsonProperty("joined_at")]
	public DateTime? JoinedAt { get; set; }

	/// <summary>
	/// Total number of members in the guild.
	/// </summary>
	[JsonProperty("member_count")]
	public int MemberCount { get; set; }

	/// <summary>
	/// Number of online members in the guild.
	/// </summary>
	[JsonProperty("online_count")]
	public int OnlineCount { get; set; }

	/// <summary>
	/// If true, the guild is temporarily unavailable due to an outage.
	/// When received on GUILD_CREATE, this indicates the guild is not yet available rather than being joined.
	/// </summary>
	[JsonProperty("unavailable")]
	public bool? Unavailable { get; set; }

	/// <summary>
	/// Nested guild properties (name, icon, owner, settings, etc.).
	/// </summary>
	[JsonProperty("properties")]
	public GuildCreateProperties? Properties { get; set; }

	/// <summary>
	/// Channels present in the guild at the time of the event.
	/// </summary>
	[JsonProperty("channels")]
	public List<ChannelGatewayData> Channels { get; set; } = new();

	/// <summary>
	/// Members present in the guild at the time of the event.
	/// </summary>
	[JsonProperty("members")]
	public List<GuildMemberGatewayData> Members { get; set; } = new();

	/// <summary>
	/// Roles defined in the guild.
	/// </summary>
	[JsonProperty("roles")]
	public List<RoleJson> Roles { get; set; } = new();

	/// <summary>
	/// Emojis defined in the guild.
	/// </summary>
	[JsonProperty("emojis")]
	public List<GuildEmojiJson> Emojis { get; set; } = new();

	/// <summary>
	/// Stickers defined in the guild.
	/// </summary>
	[JsonProperty("stickers")]
	public List<GuildStickerJson> Stickers { get; set; } = new();
}
