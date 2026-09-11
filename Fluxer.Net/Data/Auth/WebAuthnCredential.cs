namespace Fluxer.Net;

/// <inheritdoc />
public class WebAuthnCredential : Entity, IWebAuthnCredential
{
    /// <inheritdoc />
    public string Id { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? LastUsedAt { get; private set; }

    internal WebAuthnCredential(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Login object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static WebAuthnCredential Create(FluxerBaseClient client, WebAuthnCredentialJson json)
    {
        WebAuthnCredential data = new WebAuthnCredential(client);
        data.Update(json);
        return data;
    }

    internal void Update(WebAuthnCredentialJson json)
    {
        Id = json.Id;
        Name = json.Name;
        CreatedAt = json.CreatedAt;
        LastUsedAt = json.LastUsedAt;
    }
}
