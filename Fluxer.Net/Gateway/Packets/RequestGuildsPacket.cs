using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The data of a request for guilds.
/// </summary>
public class RequestGuildsPacket
{
    /// <summary>
    /// The IDs of the guilds.
    /// </summary>
    [JsonProperty("guild_ids")]
    public string[] GuildIds { get; set; }
}
