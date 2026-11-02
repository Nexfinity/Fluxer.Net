namespace Fluxer.Net;

/// <inheritdoc />
public class PermissionOverwrite : Entity, IPermissionOverwrite
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public PermissionOverwriteType Type { get; private set; }

    /// <inheritdoc />
    public ChannelPermissions Allow { get; private set; }

    /// <inheritdoc />
    public ChannelPermissions Deny { get; private set; }

    internal PermissionOverwrite(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a permission overwrite from JSON.
    /// </summary>
    public static PermissionOverwrite Create(FluxerBaseClient client, PermissionOverwriteJson json)
    {
        PermissionOverwrite data = new PermissionOverwrite(client);
        data.Update(json);
        return data;
    }

    internal void Update(PermissionOverwriteJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Allow = json.Allow;
        Deny = json.Deny;
    }
}
