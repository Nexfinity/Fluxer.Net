namespace Fluxer.Net;

/// <inheritdoc />
public class ClientStatus : Entity, IClientStatus
{
    /// <inheritdoc />
    public string? Desktop { get; private set; }

    /// <inheritdoc />
    public string? Mobile { get; private set; }

    /// <inheritdoc />
    public string? Web { get; private set; }

    internal ClientStatus(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ClientStatus object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static ClientStatus? Create(FluxerBaseClient client, ClientStatusJson? json)
    {
        if (json == null)
            return null;

        ClientStatus data = new ClientStatus(client);
        data.Update(json);
        return data;
    }

    internal void Update(ClientStatusJson json)
    {
        Desktop = json.Desktop;
        Mobile = json.Mobile;
        Web = json.Web;
    }
}
