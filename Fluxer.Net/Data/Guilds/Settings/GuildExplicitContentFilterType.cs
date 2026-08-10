namespace Fluxer.Net;

/// <summary>
/// Represents the explicit content filter level for a guild.
/// </summary>
public enum GuildExplicitContentFilterType
{
    /// <summary>
    /// Explicit content filtering is disabled (no scanning).
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Scan messages from members without roles.
    /// </summary>
    MembersWithoutRoles = 1,

    /// <summary>
    /// Scan messages from all members.
    /// </summary>
    AllMembers = 2,
}
