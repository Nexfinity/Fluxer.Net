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

    internal ClientStatus(BaseClient client) : base(client)
    {

    }

    public static ClientStatus? Create(BaseClient client, ClientStatusJson? json)
    {
        if (json == null)
            return null;

        var data = new ClientStatus(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ClientStatusJson json)
    {
        Desktop = json.Desktop;
        Mobile = json.Mobile;
        Web = json.Web;
    }
}
