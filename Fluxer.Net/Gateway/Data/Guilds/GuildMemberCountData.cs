using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The member and online counts of a guild.
/// </summary>
public class GuildMemberCountGatewayData
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The number of members in the guild.
    /// </summary>
    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    /// <summary>
    /// The number of members online.
    /// </summary>
    [JsonProperty("online_count")]
    public int OnlineCount { get; set; }
}

/// <summary>
/// The member and online counts of a channel.
/// </summary>
public class GuildChannelMemberCountGatewayData : GuildMemberCountGatewayData
{
    /// <summary>
    /// The ID of the channel.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}