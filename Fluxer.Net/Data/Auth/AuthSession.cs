namespace Fluxer.Net;

/// <inheritdoc />
public class AuthSession : Entity, IAuthSession
{
    /// <inheritdoc />
    public byte[] SessionIdHash { get; internal set; }

    /// <inheritdoc />
    public DateTime CreatedAt { get; internal set; }

    /// <inheritdoc />
    public DateTime ApproximateLastUsedAt { get; internal set; }

    /// <inheritdoc />
    public string ClientIp { get; internal set; }

    /// <inheritdoc />
    public string? ClientIpReverse { get; internal set; }

    /// <inheritdoc />
    public string? ClientOs { get; internal set; }

    /// <inheritdoc />
    public string? ClientPlatform { get; internal set; }

    /// <inheritdoc />
    public string? ClientCountry { get; internal set; }

    internal AuthSession(BaseClient client) : base(client)
    {

    }

    public static AuthSession Create(BaseClient client, AuthSessionJson json)
    {
        var data = new AuthSession(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, AuthSessionJson json)
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
