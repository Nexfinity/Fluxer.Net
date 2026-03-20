namespace Fluxer.Net;

/// <inheritdoc />
public class PermissionOverwrite : Entity, IPermissionOverwrite
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public int Type { get; internal set; }

    /// <inheritdoc />
    public ulong Allow { get; internal set; }

    /// <inheritdoc />
    public ulong Deny { get; internal set; }

    internal PermissionOverwrite(BaseClient client) : base(client)
    {

    }

    public static PermissionOverwrite Create(BaseClient client, PermissionOverwriteJson json)
    {
        var data = new PermissionOverwrite(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, PermissionOverwriteJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Allow = json.Allow;
        Deny = json.Deny;
    }
}
