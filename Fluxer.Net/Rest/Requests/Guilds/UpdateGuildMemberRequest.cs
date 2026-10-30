using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating a guild member, sending only the properties that are set.
/// </summary>
public class UpdateGuildMemberRequest
{
    [JsonProperty("nick")]
    public Optional<string?> Nickname { get; set; }

    [JsonProperty("roles")]
    public Optional<IEnumerable<ulong>> RoleIds { get; set; }

    [JsonProperty("mute")]
    public Optional<bool> IsMute { get; set; }

    [JsonProperty("deaf")]
    public Optional<bool> IsDeaf { get; set; }

    [JsonProperty("communication_disabled_until")]
    public Optional<DateTimeOffset?> CommunicationDisabledUntil { get; set; }

    [JsonProperty("timeout_reason")]
    public Optional<string?> TimeoutReason { get; set; }

    [JsonProperty("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    /// <summary>
    /// The voice connection ID of the member to move when <see cref="ChannelId"/> is set.
    /// </summary>
    [JsonProperty("connection_id")]
    public Optional<string?> ConnectionId { get; set; }
}
