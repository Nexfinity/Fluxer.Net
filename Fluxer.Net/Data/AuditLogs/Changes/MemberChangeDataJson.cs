using Newtonsoft.Json;

namespace Fluxer.Net;

public class MemberChangeDataJson : IAuditLogData
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("nick")]
    public string? Nickname { get; set; }

    [JsonProperty("roles")]
    public HashSet<ulong>? RoleIds { get; set; } = new HashSet<ulong>();

    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    [JsonProperty("bio")]
    public string? Bio { get; set; }

    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    [JsonProperty("deaf")]
    public bool? IsDeaf { get; set; }

    [JsonProperty("mute")]
    public bool? IsMute { get; set; }

    [JsonProperty("communication_disabled_until")]
    public DateTimeOffset? CommunicationDisabledUntil { get; set; }
}
