namespace Fluxer.Net;

public interface IGuildMember : ISnowflake
{
    /// <summary>
    /// Guild id that the user is in.
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    /// Get the mention for the user.
    /// </summary>
    string Mention { get; }

    /// <summary>
    /// User data for the member.
    /// </summary>
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

    /// <summary>
    /// The accent colour of the member guild profile as an integer.
    /// </summary>
    int? AccentColor { get; }

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
    HashSet<ulong> RoleIds { get; }

    /// <summary>
    /// Get the members's current nickname, display name or username.
    /// </summary>
    string GetCurrentName();

    /// <summary>
    /// Get the default avatar for the user.
    /// </summary>
    string GetDefaultAvatarUrl();

    /// <summary>
    /// Get the members's avatar.
    /// </summary>
    string? GetAvatarUrl(int size);

    /// <summary>
    /// Get the members's avatar or fallback to default.
    /// </summary>
    string GetAvatarOrDefaultUrl(int size);

    /// <summary>
    /// Get the member's banner.
    /// </summary>
    string? GetBannerUrl(int size);
}
