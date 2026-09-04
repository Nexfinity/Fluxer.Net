namespace Fluxer.Net;

public class SocketRole : Role
{
    public SocketGuild Guild { get; private set; }

    public bool HasPermission(GuildPermission permission)
    {
        if (Permissions.RawValue.HasFlag(permission))
            return true;

        if (Guild.Permissions.RawValue.HasFlag(permission))
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
        SocketRole data = new SocketRole(client)
        {
            GuildId = guild.Id,
            Guild = guild
        };
        data.Update(json);
        return data;
    }

    internal override void Update(RoleJson json)
    {
        base.Update(json);
    }
}
