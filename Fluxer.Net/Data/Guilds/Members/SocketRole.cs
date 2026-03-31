namespace Fluxer.Net;

public class SocketRole : Role
{
    internal SocketRole(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketRole Create(FluxerBaseClient client, RoleJson json, ulong guildId)
    {
        SocketRole data = new SocketRole(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, RoleJson json)
    {
        base.Update(client, json);
    }
}
