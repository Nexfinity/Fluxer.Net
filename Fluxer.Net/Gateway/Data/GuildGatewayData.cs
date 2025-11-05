using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway guild data matching the GuildResponse API model
/// </summary>
public class GuildGatewayData : IGatewayData
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
