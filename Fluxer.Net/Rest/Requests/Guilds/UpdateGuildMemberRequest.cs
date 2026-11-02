using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating a guild member, sending only the properties that are set.
/// </summary>
public class UpdateGuildMemberRequest
{
    /// <summary>
    /// The member's nickname, 1 to 32 characters, or null to clear it.
    /// </summary>
    [JsonProperty("nick")]
    public Optional<string?> Nickname { get; set; }

    /// <summary>
    /// The IDs of the roles the member has.
    /// </summary>
    [JsonProperty("roles")]
    public Optional<IEnumerable<ulong>> RoleIds { get; set; }

    /// <summary>
    /// Base64 image data for the member's guild avatar, or null to remove it.
    /// </summary>
    [JsonProperty("avatar")]
    public Optional<string?> AvatarBase64 { get; set; }

    /// <summary>
    /// Base64 image data for the member's guild banner, or null to remove it.
    /// </summary>
    [JsonProperty("banner")]
    public Optional<string?> BannerBase64 { get; set; }

    /// <summary>
    /// The member's guild profile bio, 1 to 320 characters, or null to clear it.
    /// </summary>
    [JsonProperty("bio")]
    public Optional<string?> Bio { get; set; }

    /// <summary>
    /// The member's guild profile pronouns, 1 to 40 characters, or null to clear them.
    /// </summary>
    [JsonProperty("pronouns")]
    public Optional<string?> Pronouns { get; set; }

    /// <summary>
    /// The member's guild profile accent color as an RGB integer, or null to clear it.
    /// </summary>
    [JsonProperty("accent_color")]
    public Optional<int?> AccentColor { get; set; }

    /// <summary>
    /// Flags on the member's guild profile.
    /// </summary>
    [JsonProperty("profile_flags")]
    public Optional<GuildMemberProfileFlags?> ProfileFlags { get; set; }

    /// <summary>
    /// Whether replies to the member mention them by default in this guild.
    /// </summary>
    [JsonProperty("mention_flags")]
    public Optional<MentionReplyPreference?> MentionFlags { get; set; }

    /// <summary>
    /// Whether the member is muted in voice channels.
    /// </summary>
    [JsonProperty("mute")]
    public Optional<bool> IsMute { get; set; }

    /// <summary>
    /// Whether the member is deafened in voice channels.
    /// </summary>
    [JsonProperty("deaf")]
    public Optional<bool> IsDeaf { get; set; }

    /// <summary>
    /// When the member's timeout ends, or null to remove the timeout.
    /// </summary>
    [JsonProperty("communication_disabled_until")]
    public Optional<DateTimeOffset?> CommunicationDisabledUntil { get; set; }

    /// <summary>
    /// The reason for the timeout, or null to clear it.
    /// </summary>
    [JsonProperty("timeout_reason")]
    public Optional<string?> TimeoutReason { get; set; }

    /// <summary>
    /// The voice channel to move the member to, or null to disconnect.
    /// </summary>
    [JsonProperty("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    /// <summary>
    /// The voice connection ID of the member to move when <see cref="ChannelId"/> is set.
    /// </summary>
    [JsonProperty("connection_id")]
    public Optional<string?> ConnectionId { get; set; }
}
