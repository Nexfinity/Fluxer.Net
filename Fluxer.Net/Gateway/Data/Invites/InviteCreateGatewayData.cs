using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The INVITE_CREATE payload: an invite with the IDs of its guild and channel.
/// </summary>
public class InviteCreateGatewayData : InviteJson
{
    /// <summary>
    /// The ID of the guild the invite is for.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <summary>
    /// The ID of the channel the invite is for.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }
}
