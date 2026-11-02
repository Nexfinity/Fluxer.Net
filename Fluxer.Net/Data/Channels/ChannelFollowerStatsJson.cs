using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The number of channels and guilds that follow an announcement channel.
/// </summary>
public class ChannelFollowerStatsJson
{
    /// <summary>
    /// The number of channels following the announcement channel.
    /// </summary>
    [JsonProperty("channel_count")]
    public int ChannelCount { get; set; }

    /// <summary>
    /// The number of different guilds following the announcement channel.
    /// </summary>
    [JsonProperty("guild_count")]
    public int GuildCount { get; set; }
}
