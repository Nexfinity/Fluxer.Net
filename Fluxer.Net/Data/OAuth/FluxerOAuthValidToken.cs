namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthValidToken : IFluxerOAuthValidToken
{
    /// <inheritdoc />
    public bool IsActive { get; internal set; }

    /// <inheritdoc />
    public string? Scope { get; internal set; }

    /// <inheritdoc />
    public ulong? ClientId { get; internal set; }

    /// <inheritdoc />
    public string? TokenType { get; internal set; }

    /// <inheritdoc />
    public int? Exp { get; internal set; }

    /// <inheritdoc />
    public int? Iat { get; internal set; }

    /// <inheritdoc />
    public ulong? Sub { get; internal set; }
}
