namespace Fluxer.Net;

/// <summary>
/// Represents the NSFW (Not Safe For Work) filter level for a user.
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
