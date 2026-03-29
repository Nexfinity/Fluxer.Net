namespace Fluxer.Net;

public interface IGuildMember
{
    /// <summary>
    /// Guild id that the user is in.
    /// </summary>
    ulong GuildId { get; }

    IUser User { get; }

    /// <summary>
    /// ISO8601 timestamp of when the user joined the guild.
    /// </summary>
    DateTime JoinedAt { get; }

    /// <summary>
    /// The nickname of the member in this guild.
    /// </summary>
    string? Nickname { get; }

    /// <summary>
    /// The hash of the member guild-specific avatar.
    /// </summary>
    string? AvatarHash { get; }

    /// <summary>
    /// The hash of the member guild-specific banner.
    /// </summary>
    string? BannerHash { get; }

    string? Bio { get; }

    string? Pronouns { get; }

    /// <summary>
    /// The accent colour of the member guild profile as an integer.
    /// </summary>
    int? AccentColor { get; }

    JoinSource? JoinSourceType { get; }

    string? SourceInviteCode { get; }

    ulong? InviterId { get; }

    /// <summary>
    /// Whether the member is deafened in voice channels.
    /// </summary>
    bool IsDeaf { get; }

    /// <summary>
    /// Whether the member is muted in voice channels.
    /// </summary>
    bool IsMute { get; }

    /// <summary>
    /// ISO8601 timestamp until which the member is timed out.
    /// </summary>
    DateTime? CommunicationDisabledUntil { get; }

    /// <summary>
    /// Array of role IDs the member has.
    /// </summary>
    HashSet<ulong>? RoleIds { get; }

    bool IsPremiumSanitized { get; }

    bool IsTemporary { get; }
}
