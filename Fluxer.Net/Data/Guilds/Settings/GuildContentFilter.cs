namespace Fluxer.Net;

/// <summary>
/// The NSFW content filter level for the members of a guild.
/// </summary>
public enum GuildContentFilter
{
    /// <summary>
    /// NSFW filtering is disabled.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Filter NSFW content from members that don't have a role.
    /// </summary>
    NonRoles = 1,

    /// <summary>
    /// Filter NSFW content from all members.
    /// </summary>
    Everyone = 2,
}
