using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Messages;

/// <summary>
/// Gateway message data matching the MessageResponse API model
/// </summary>
public class MessageGatewayData : MessageJson
{
    /// <summary>
    /// The type of the channel.
    /// </summary>
    [JsonProperty("channel_type")]
    public ChannelType ChannelType { get; set; }

    /// <summary>
    /// Guild member data for the author (only present in guild messages)
    /// </summary>
    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }

    /// <summary>
    /// ID of the guild where the message was sent (null for DMs)
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}