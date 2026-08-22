namespace Fluxer.Net;

public interface IFluxerOAuthToken
{
    /// <summary>
    /// The application associated with the token.
    /// </summary>
    IPartialApplication Application { get; }

    /// <summary>
    /// The list of granted OAuth2 scopes.
    /// </summary>
    string[] Scopes { get; }

    /// <summary>
    /// The expiration timestamp of the token.
    /// </summary>
    DateTime ExpiresAt { get; }

    /// <summary>
    /// The user associated with the token.
    /// </summary>
    IFluxerOAuthUser User { get; }
}
