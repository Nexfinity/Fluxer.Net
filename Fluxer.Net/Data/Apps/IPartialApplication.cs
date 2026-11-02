namespace Fluxer.Net;

/// <summary>
/// The fields shared by partial and full application objects.
/// </summary>
public interface IPartialApplication : ISnowflake
{
    /// <summary>
    /// The name of the application.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The icon hash of the application.
    /// </summary>
    string Icon { get; }

    /// <summary>
    /// The description of the application.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Whether the bot can be invited by anyone.
    /// </summary>
    bool IsPublic { get; }

    /// <summary>
    /// Whether the bot requires OAuth2 code grant.
    /// </summary>
    bool RequiresCodeGrant { get; }

    /// <summary>
    /// The application flags as a bit field.
    /// </summary>
    ulong Flags { get; }
}
