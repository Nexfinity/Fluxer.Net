namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthToken : Entity, IFluxerOAuthToken
{
    /// <inheritdoc />
    public PartialApplication Application { get; internal set; }

    /// <inheritdoc />
    public string[] Scopes { get; internal set; }

    /// <inheritdoc />
    public DateTimeOffset ExpiresAt { get; internal set; }

    /// <inheritdoc />
    public FluxerOAuthUser User { get; internal set; }

    IPartialApplication IFluxerOAuthToken.Application => Application;

    IFluxerOAuthUser IFluxerOAuthToken.User => User;

    internal FluxerOAuthToken(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a FluxerOAuthToken object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static FluxerOAuthToken Create(FluxerBaseClient client, FluxerOAuthTokenJson json)
    {
        return new FluxerOAuthToken(client)
        {
            Application = PartialApplication.Create(client, json.Application),
            Scopes = json.Scopes,
            ExpiresAt = json.ExpiresAt,
            User = FluxerOAuthUser.Create(client, json.User)
        };
    }
}
