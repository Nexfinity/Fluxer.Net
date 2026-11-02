using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The data of a request for the channels of a guild.
/// </summary>
public class RequestGuildChannelsPacket
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [JsonProperty("guild_id")]
    public string GuildId { get; set; }

    /// <summary>
    /// The IDs of the channels.
    /// </summary>
    [JsonProperty("channel_ids")]
    public string[] ChannelIds { get; set; }
}
