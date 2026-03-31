namespace Fluxer.Net;

/// <summary>
/// Cached guild from the gateway.
/// </summary>
public class SocketGuild : Guild
{
    /// <summary>
    /// Cached current logged in member for the guild.
    /// </summary>
    public SocketGuildMember CurrentMember { get; internal set; }

    internal SocketGuild(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketGuild Create(FluxerBaseClient client, GuildJson json, SocketGuildMember member)
    {
        SocketGuild data = new SocketGuild(client);
        data.CurrentMember = member;
        data.CurrentMember.Guild = data;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildJson json)
    {
        base.Update(client, json);
    }
}