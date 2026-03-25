using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildMemberJson : IGuildMember
{
    public ulong UserId => User.Id;

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public User User { get; set; }

    /// <inheritdoc />
    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("nick")]
    public string? Nickname { get; set; }

    /// <inheritdoc />
    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <inheritdoc />
    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    /// <inheritdoc />
    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    /// <inheritdoc />
    [JsonProperty("join_source_type")]
    public int? JoinSourceType { get; set; }

    /// <inheritdoc />
    [JsonProperty("source_invite_code")]
    public string? SourceInviteCode { get; set; }

    /// <inheritdoc />
    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }

    /// <inheritdoc />
    [JsonProperty("deaf")]
    public bool IsDeaf { get; set; }

    /// <inheritdoc />
    [JsonProperty("mute")]
    public bool IsMute { get; set; }

    /// <inheritdoc />
    [JsonProperty("communication_disabled_until")]
    public DateTime? CommunicationDisabledUntil { get; set; }

    /// <inheritdoc />
    [JsonProperty("roles")]
    public HashSet<ulong>? RoleIds { get; set; }

    /// <inheritdoc />
    [JsonProperty("is_premium_sanitized")]
    public bool IsPremiumSanitized { get; set; }

    /// <inheritdoc />
    [JsonProperty("temporary")]
    public bool IsTemporary { get; set; }

    IUser IGuildMember.User => User;
}
