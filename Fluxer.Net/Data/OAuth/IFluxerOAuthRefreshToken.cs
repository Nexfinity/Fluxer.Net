namespace Fluxer.Net;

/// <summary>
/// Exchange a refresh token for a new access token.
/// </summary>
public interface IFluxerOAuthRefreshToken
{
    /// <summary>
    /// New access token.
    /// </summary>
    string AccessToken { get; }

    /// <summary>
    /// Token type.
    /// </summary>
    string TokenType { get; }

    /// <summary>
    /// Expires in 
    /// </summary>
    int ExpiresIn { get; }

    /// <summary>
    /// New refresh token.
    /// </summary>
    string RefreshToken { get; }

    /// <summary>
    /// Scope of the refresh token.
    /// </summary>
    string Scope { get; }
}
