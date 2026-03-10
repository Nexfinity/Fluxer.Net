using Fluxer.Net.Data.Users;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for USER_GUILD_SETTINGS_UPDATE event when user guild settings are updated.
/// </summary>
public class UserGuildSettingsUpdateGatewayData : IGatewayData
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("settings")]
	public UserGuildSettings Settings { get; set; } = null!;
}
