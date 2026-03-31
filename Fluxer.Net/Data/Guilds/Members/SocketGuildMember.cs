namespace Fluxer.Net;

public class SocketGuildMember : GuildMember
{
    public SocketGuild Guild { get; internal set; }

    internal SocketGuildMember(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketGuildMember Create(FluxerBaseClient client, GuildMemberJson json)
    {
        SocketGuildMember data = new SocketGuildMember(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildMemberJson json)
    {
        base.Update(client, json);
    }
}