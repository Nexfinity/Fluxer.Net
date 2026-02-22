using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/channel/ChannelSchemas.tsx#L51"/>
/// </remarks>
public class Channel
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("topic")]
	public string? Topic { get; set; }

	[JsonProperty("icon")]
	public string? IconHash { get; set; }

	[JsonProperty("url")]
	public string? Url { get; set; }

	[JsonProperty("parent_id")]
	public ulong? ParentId { get; set; }

	[JsonProperty("position")]
	public int Position { get; set; }

	[JsonProperty("owner_id")]
	public ulong? OwnerId { get; set; }

	[JsonProperty("recipient_ids")]
	public HashSet<ulong>? RecipientIds { get; set; }

	[JsonProperty("nsfw")]
	public bool IsNsfw { get; set; }

	[JsonProperty("rate_limit_per_user")]
	public int RateLimitPerUser { get; set; }

	[JsonProperty("bitrate")]
	public int? Bitrate { get; set; }

	[JsonProperty("user_limit")]
	public int? UserLimit { get; set; }

	[JsonProperty("rtc_region")]
	public string? RtcRegion { get; set; }

	[JsonProperty("last_message_id")]
	public ulong? LastMessageId { get; set; }

	[JsonProperty("last_pin_timestamp")]
	public DateTime? LastPinTimestamp { get; set; }

	[JsonProperty("permission_overwrites")]
	public List<ChannelPermissionOverwrite>? PermissionOverwrites { get; set; }

	[JsonProperty("nicks")]
	public Dictionary<string, string>? Nicknames { get; set; }

	[JsonProperty("soft_deleted")]
	public bool IsSoftDeleted { get; set; }

	[JsonProperty("indexed_at")]
	public DateTime? IndexedAt { get; set; }
}
