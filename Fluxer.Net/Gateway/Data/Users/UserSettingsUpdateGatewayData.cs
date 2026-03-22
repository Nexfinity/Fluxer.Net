using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Users;

/// <summary>
/// Gateway data for USER_SETTINGS_UPDATE event when user settings are updated.
/// </summary>
public class UserSettingsUpdateGatewayData
{
    [JsonProperty("settings")]
    public UserSettingsJson Settings { get; set; } = null!;
}
