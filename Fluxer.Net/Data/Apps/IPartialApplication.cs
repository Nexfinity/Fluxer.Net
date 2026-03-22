namespace Fluxer.Net.Data.Apps;

/// <summary>
/// Application/bot used to interact with the Fluxer platform and API.
/// </summary>
public interface IPartialApplication
{
    /// <summary>
    /// The unique identifier of the application.
    /// </summary>
    ulong Id { get; }

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
    /// The application flags.
    /// </summary>
    ulong Flags { get; }
}
