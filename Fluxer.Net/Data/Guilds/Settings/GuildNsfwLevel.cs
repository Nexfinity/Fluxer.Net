namespace Fluxer.Net;

/// <summary>
/// Represents the nsfw level required for members to participate in a guild.
/// </summary>
public enum GuildNsfwLevel
{
    /// <summary>
    /// Default verification level.
    /// </summary>
    Default = 0,

    /// <summary>
    /// May contain some adults topics but not overly nsfw.
    /// </summary>
    Explicit = 1,

    /// <summary>
    /// Safe for all user.
    /// </summary>
    Safe = 2,

    /// <summary>
    /// Users in certain countries will need to be age verified.
    /// </summary>
    AgeRestricted = 3
}