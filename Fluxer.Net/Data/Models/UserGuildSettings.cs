using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class UserGuildSettings
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("message_notifications")]
	public int? MessageNotifications { get; set; }

	[JsonProperty("muted")]
	public bool Muted { get; set; }

	[JsonProperty("mute_config")]
	public MuteConfiguration? MuteConfig { get; set; }

	[JsonProperty("mobile_push")]
	public bool MobilePush { get; set; }

	[JsonProperty("suppress_everyone")]
	public bool SuppressEveryone { get; set; }

	[JsonProperty("suppress_roles")]
	public bool SuppressRoles { get; set; }

	[JsonProperty("hide_muted_channels")]
	public bool HideMutedChannels { get; set; }

	[JsonProperty("channel_overrides")]
	public Dictionary<ulong, GuildChannelOverride>? ChannelOverrides { get; set; }

	[JsonProperty("version")]
	public int Version { get; set; }
}
