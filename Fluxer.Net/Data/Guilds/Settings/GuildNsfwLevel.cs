namespace Fluxer.Net;

/// <summary>
/// The NSFW level of a guild.
/// </summary>
public enum GuildNsfwLevel
{
    /// <summary>
    /// Safe for all users. Fluxer's only other level is <see cref="AgeRestricted"/>.
    /// </summary>
    Default = 0,

    /// <summary>
    /// A legacy value that Fluxer reads as <see cref="AgeRestricted"/>.
    /// </summary>
    Explicit = 1,

    /// <summary>
    /// A legacy value that Fluxer reads as <see cref="Default"/>.
    /// </summary>
    Safe = 2,

    /// <summary>
    /// Users in certain countries will need to be age verified.
    /// </summary>
    AgeRestricted = 3
}