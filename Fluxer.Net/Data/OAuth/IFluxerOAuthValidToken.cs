namespace Fluxer.Net;

/// <summary>
/// The result of validating an OAuth2 access token.
/// </summary>
public interface IFluxerOAuthValidToken
{
    /// <summary>
    /// Whether the token is active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// The scopes of the token.
    /// </summary>
    string? Scope { get; }

    /// <summary>
    /// The id of the application the token was issued to.
    /// </summary>
    ulong? ClientId { get; }

    /// <summary>
    /// The type of the token.
    /// </summary>
    string? TokenType { get; }

    /// <summary>
    /// The expiration time of the token, as a Unix timestamp in seconds.
    /// </summary>
    int? Exp { get; }

    /// <summary>
    /// The time the token was issued, as a Unix timestamp in seconds.
    /// </summary>
    int? Iat { get; }

    /// <summary>
    /// The id of the user the token was issued for.
    /// </summary>
    ulong? Sub { get; }
}
