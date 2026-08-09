namespace Fluxer.Net;

public class SocketRole : Role
{
    public SocketGuild Server { get; internal set; }

    public bool HasPermission(GuildPermission permission)
    {
        if (Permissions.RawValue.HasFlag(permission))
            return true;

        if (Server.Permissions.RawValue.HasFlag(permission))
            return true;

        return false;
    }

    internal SocketRole(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketRole object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public static SocketRole Create(FluxerBaseClient client, RoleJson json, SocketGuild guild)
    {
        SocketRole data = new SocketRole(client);
        data.GuildId = guild.Id;
        data.Server = guild;
        data.Update(client, json);
        return data;
    }

    internal override void Update(FluxerBaseClient client, RoleJson json)
    {
        base.Update(client, json);
    }
}
