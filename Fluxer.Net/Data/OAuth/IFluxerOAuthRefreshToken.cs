namespace Fluxer.Net;

/// <summary>
/// The result of exchanging a refresh token for a new access token.
/// </summary>
public interface IFluxerOAuthRefreshToken
{
    /// <summary>
    /// The new access token.
    /// </summary>
    string AccessToken { get; }

    /// <summary>
    /// The type of the token.
    /// </summary>
    string TokenType { get; }

    /// <summary>
    /// The number of seconds until the access token expires.
    /// </summary>
    int ExpiresIn { get; }

    /// <summary>
    /// The new refresh token.
    /// </summary>
    string RefreshToken { get; }

    /// <summary>
    /// Scope of the refresh token.
    /// </summary>
    string Scope { get; }
}
