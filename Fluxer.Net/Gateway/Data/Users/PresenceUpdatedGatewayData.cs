using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway event data for PRESENCE_UPDATE events.
/// This differs from the base Presence class by including a full User object instead of just a user_id.
/// </summary>
public class PresenceGatewayData
{
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("activities")]
    public List<ActivityJson>? Activities { get; set; }

    [JsonProperty("client_status")]
    public ClientStatusJson? ClientStatus { get; set; }

    /// <summary>
    /// Whether the user is AFK.
    /// </summary>
    [JsonProperty("afk")]
    public bool? Afk { get; set; }

    /// <summary>
    /// Whether the user is on mobile.
    /// </summary>
    [JsonProperty("mobile")]
    public bool? Mobile { get; set; }
}
