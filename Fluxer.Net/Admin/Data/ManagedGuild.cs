namespace Fluxer.Net.Admin;

public class ManagedGuild : Guild
{
    internal ManagedGuild(FluxerBaseClient client) : base(client)
    {

    }

    public static ManagedGuild Create(FluxerAdminClient client, GuildJson json)
    {
        ManagedGuild data = new ManagedGuild(client);
        data.Update(json);
        return data;
    }
}
