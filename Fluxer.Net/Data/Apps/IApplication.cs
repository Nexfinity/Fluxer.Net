namespace Fluxer.Net;

/// <summary>
/// An application with its OAuth2 redirect URIs and bot user.
/// </summary>
public interface IApplication : IPartialApplication
{
    /// <summary>
    /// The registered redirect URIs for OAuth2.
    /// </summary>
    string[] RedirectUrls { get; }

    /// <summary>
    /// The bot user of the application.
    /// </summary>
    IUser Bot { get; }
}
