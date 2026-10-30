namespace Fluxer.Net;

/// <summary>
/// Flags on a member's guild profile.
/// </summary>
[Flags]
public enum GuildMemberProfileFlags
{
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// The member's guild avatar is unset and their user avatar shows.
    /// </summary>
    AvatarUnset = 1 << 0,

    /// <summary>
    /// The member's guild banner is unset and their user banner shows.
    /// </summary>
    BannerUnset = 1 << 1
}
