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
    public GuildMemberGatewayData? Member { get; set; }

    /// <summary>
    /// ID of the guild where the message was sent (null for DMs)
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}

/// <summary>
/// Partial guild member response for message context
/// </summary>
public class GuildMemberGatewayData
{
    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    [JsonProperty("communication_disabled_until")]
    public DateTime? CommunicationDisabledUntil { get; set; }

    [JsonProperty("nick")]
    public string? Nick { get; set; }

    [JsonProperty("roles")]
    public List<ulong> Roles { get; set; } = new();

    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("deaf")]
    public bool IsDeaf { get; set; }

    [JsonProperty("mute")]
    public bool IsMute { get; set; }
}
