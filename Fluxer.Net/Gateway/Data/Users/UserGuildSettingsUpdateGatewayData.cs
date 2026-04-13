using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Users;

/// <summary>
/// Gateway data for USER_GUILD_SETTINGS_UPDATE event when user guild settings are updated.
/// </summary>
public class UserGuildSettingsUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("settings")]
    public UserGuildSettingsJson Settings { get; set; } = null!;
}
