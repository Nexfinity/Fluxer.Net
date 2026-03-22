using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Guilds;

/// <summary>
/// Gateway data for GUILD_DELETE event.
/// Sent when a guild becomes unavailable during a guild outage, or when the user leaves or is removed from a guild.
/// </summary>
public class GuildDeleteGatewayData
{
    /// <summary>
    /// ID of the guild that was deleted or became unavailable.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// If true, the guild became unavailable due to an outage and may become available again later.
    /// If false or null, the user was removed from the guild (left, kicked, or banned).
    /// </summary>
    [JsonProperty("unavailable")]
    public bool? Unavailable { get; set; }
}
