using Newtonsoft.Json;

namespace Fluxer.Net.Data.Guilds;

public class GuildChannelOverride
{
	[JsonProperty("collapsed")]
	public bool Collapsed { get; set; }

	[JsonProperty("message_notifications")]
	public int? MessageNotifications { get; set; }

	[JsonProperty("muted")]
	public bool Muted { get; set; }

	[JsonProperty("mute_config")]
	public MuteConfiguration? MuteConfig { get; set; }
}
