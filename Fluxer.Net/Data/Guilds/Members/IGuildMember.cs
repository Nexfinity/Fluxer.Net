namespace Fluxer.Net;

/// <summary>
/// A member of a guild.
/// </summary>
public interface IGuildMember : ISnowflake, IMentionable
{
    /// <summary>
    /// The ID of the guild the member is in.
    /// </summary>
    ulong GuildId { get; }
    /// <summary>
    /// The user account of the member.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// When the user joined the guild.
    /// </summary>
    DateTimeOffset JoinedAt { get; }

    /// <summary>
    /// The nickname of the member in this guild.
    /// </summary>
    string? Nickname { get; }

    /// <summary>
    /// The hash of the member's guild avatar.
    /// </summary>
    string? AvatarHash { get; }

    /// <summary>
    /// The hash of the member's guild banner.
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
    /// When the member's timeout ends, or null when the member is not timed out.
    /// </summary>
    DateTimeOffset? CommunicationDisabledUntil { get; }

    /// <summary>
    /// The IDs of the roles the member has.
    /// </summary>
    HashSet<ulong> RoleIds { get; }

    /// <summary>
    /// Gets the member's nickname, display name or username.
    /// </summary>
    string GetCurrentName();

    /// <summary>
    /// Gets the default avatar URL for the user.
    /// </summary>
    string GetDefaultAvatarUrl();

    /// <summary>
    /// Gets the member's guild avatar URL, or their account avatar URL when they have no guild avatar.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetAvatarUrl(int size);

    /// <summary>
    /// Gets the member's guild avatar URL, or null when they have no guild avatar.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetGuildAvatarUrl(int size);

    /// <summary>
    /// Gets the member's avatar URL, or the default avatar URL when they have no avatar.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string GetAvatarOrDefaultUrl(int size);

    /// <summary>
    /// Gets the member's guild banner URL, or null when they have no guild banner.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    string? GetBannerUrl(int size);
}
