namespace Fluxer.Net;

/// <summary>
/// Application/bot used to interact with the Fluxer platform and API.
/// </summary>
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
