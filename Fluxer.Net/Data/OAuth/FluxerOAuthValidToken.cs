namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthValidToken : Entity, IFluxerOAuthValidToken
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

    internal FluxerOAuthValidToken(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a FluxerOAuthValidToken object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static FluxerOAuthValidToken Create(FluxerBaseClient client, FluxerOAuthValidTokenJson json)
    {
        return new FluxerOAuthValidToken(client)
        {
            IsActive = json.IsActive,
            Scope = json.Scope,
            ClientId = json.ClientId,
            TokenType = json.TokenType,
            Exp = json.Exp,
            Iat = json.Iat,
            Sub = json.Sub,
        };
    }
}
