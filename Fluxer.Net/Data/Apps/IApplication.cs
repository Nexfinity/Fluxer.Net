namespace Fluxer.Net;

public interface IApplication
{
    /// <summary>
    /// The registered redirect URIs for OAuth2.
    /// </summary>
    /// <remarks>
    /// Maximum 20
    /// </remarks>
    string[] RedirectUrls { get; }

    /// <summary>
    /// Detailed bot user metadata.
    /// </summary>
    IUser Bot { get; }
}
