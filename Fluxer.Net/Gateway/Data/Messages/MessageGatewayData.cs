using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A message as the gateway sends it, with the guild and the author's member.
/// </summary>
public class MessageGatewayData : MessageJson
{
    /// <summary>
    /// The type of the channel the message is in.
    /// </summary>
    [JsonProperty("channel_type")]
    public ChannelType ChannelType { get; set; }

    /// <summary>
    /// The author's guild member, for guild messages.
    /// </summary>
    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }

    /// <summary>
    /// The guild the message is in, or null for direct messages.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}