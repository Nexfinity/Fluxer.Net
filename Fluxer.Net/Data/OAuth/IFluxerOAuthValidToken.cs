namespace Fluxer.Net;

public interface IFluxerOAuthValidToken
{
    /// <summary>
    /// Is token valid.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Scope of the token.
    /// </summary>
    string? Scope { get; }

    /// <summary>
    /// Client ID for the application used.
    /// </summary>
    ulong? ClientId { get; }

    /// <summary>
    /// Token type.
    /// </summary>
    string? TokenType { get; }

    int? Exp { get; }

    int? Iat { get; }

    ulong? Sub { get; }
}
