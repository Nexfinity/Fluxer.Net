namespace Fluxer.Net;

/// <inheritdoc />
public class ClientStatus : Entity, IClientStatus
{
    /// <inheritdoc />
    public string? Desktop { get; internal set; }

    /// <inheritdoc />
    public string? Mobile { get; internal set; }

    /// <inheritdoc />
    public string? Web { get; internal set; }

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
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ClientStatusJson json)
    {
        Desktop = json.Desktop;
        Mobile = json.Mobile;
        Web = json.Web;
    }
}
