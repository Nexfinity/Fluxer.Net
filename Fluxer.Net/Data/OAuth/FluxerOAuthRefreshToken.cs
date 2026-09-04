namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthRefreshToken : Entity, IFluxerOAuthRefreshToken
{
    /// <inheritdoc />
    public string AccessToken { get; private set; }

    /// <inheritdoc />
    public string TokenType { get; private set; }

    /// <inheritdoc />
    public int ExpiresIn { get; private set; }

    /// <inheritdoc />
    public string RefreshToken { get; private set; }

    /// <inheritdoc />
    public string Scope { get; private set; }

    internal FluxerOAuthRefreshToken(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a FluxerOAuthRefreshToken object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static FluxerOAuthRefreshToken Create(FluxerBaseClient client, FluxerOAuthRefreshTokenJson json)
    {
        FluxerOAuthRefreshToken data = new FluxerOAuthRefreshToken(client)
        {
            AccessToken = json.AccessToken,
            TokenType = json.TokenType,
            ExpiresIn = json.ExpiresIn,
            RefreshToken = json.RefreshToken,
            Scope = json.Scope
        };
        return data;
    }
}