namespace Fluxer.Net;

/// <inheritdoc />
public class PermissionOverwrite : Entity, IPermissionOverwrite
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public int Type { get; internal set; }

    /// <inheritdoc />
    public ChannelPermissions Allow { get; internal set; }

    /// <inheritdoc />
    public ChannelPermissions Deny { get; internal set; }

    internal PermissionOverwrite(FluxerBaseClient client) : base(client)
    {

    }

    public static PermissionOverwrite Create(FluxerBaseClient client, PermissionOverwriteJson json)
    {
        PermissionOverwrite data = new PermissionOverwrite(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, PermissionOverwriteJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Allow = json.Allow;
        Deny = json.Deny;
    }
}
