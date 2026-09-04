namespace Fluxer.Net;

/// <inheritdoc />
public class AuthSession : Entity, IAuthSession
{
    /// <inheritdoc />
    public byte[] SessionIdHash { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset ApproximateLastUsedAt { get; private set; }

    /// <inheritdoc />
    public string ClientIp { get; private set; }

    /// <inheritdoc />
    public string? ClientIpReverse { get; private set; }

    /// <inheritdoc />
    public string? ClientOs { get; private set; }

    /// <inheritdoc />
    public string? ClientPlatform { get; private set; }

    /// <inheritdoc />
    public string? ClientCountry { get; private set; }

    internal AuthSession(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a AuthSession object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static AuthSession Create(FluxerBaseClient client, AuthSessionJson json)
    {
        AuthSession data = new AuthSession(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, AuthSessionJson json)
    {
        SessionIdHash = json.SessionIdHash;
        CreatedAt = json.CreatedAt;
        ApproximateLastUsedAt = json.ApproximateLastUsedAt;
        ClientIp = json.ClientIp;
        ClientIpReverse = json.ClientIpReverse;
        ClientOs = json.ClientOs;
        ClientPlatform = json.ClientPlatform;
        ClientCountry = json.ClientCountry;
    }
}
