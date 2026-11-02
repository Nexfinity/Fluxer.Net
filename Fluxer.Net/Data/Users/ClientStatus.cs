namespace Fluxer.Net;

/// <summary>
/// The status of a user on each client platform.
/// </summary>
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
    /// Creates a client status from its JSON data.
    /// </summary>
    /// <param name="client">The client the status belongs to.</param>
    /// <param name="json">The status data, or null when there is none.</param>
    /// <returns>The client status, or null when <paramref name="json"/> is null.</returns>
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
