using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class MemberChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the member's user.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The nickname of the member.
    /// </summary>
    [JsonProperty("nick")]
    public string? Nickname { get; set; }

    /// <summary>
    /// The ids of the member's roles.
    /// </summary>
    [JsonProperty("roles")]
    public HashSet<ulong>? RoleIds { get; set; } = new HashSet<ulong>();

    /// <summary>
    /// The avatar hash of the member.
    /// </summary>
    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    /// <summary>
    /// The banner hash of the member.
    /// </summary>
    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    /// <summary>
    /// The bio of the member.
    /// </summary>
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <summary>
    /// The pronouns of the member.
    /// </summary>
    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    /// <summary>
    /// The accent color of the member.
    /// </summary>
    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    /// <summary>
    /// Whether the member is server deafened.
    /// </summary>
    [JsonProperty("deaf")]
    public bool? IsDeaf { get; set; }

    /// <summary>
    /// Whether the member is server muted.
    /// </summary>
    [JsonProperty("mute")]
    public bool? IsMute { get; set; }

    /// <summary>
    /// When the member's timeout ends.
    /// </summary>
    [JsonProperty("communication_disabled_until")]
    public DateTimeOffset? CommunicationDisabledUntil { get; set; }
}
