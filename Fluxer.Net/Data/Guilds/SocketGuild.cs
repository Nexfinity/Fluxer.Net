using System.Collections.Concurrent;

namespace Fluxer.Net;

/// <summary>
/// Cached guild from the gateway.
/// </summary>
public class SocketGuild : Guild
{
    internal TaskCompletionSource<bool>? _downloaderPromise;

    /// <summary>
    /// Cached current logged in member for the guild.
    /// </summary>
    public SocketGuildMember CurrentMember { get; internal set; }

    public SocketRole EveryoneRole => Roles.GetValueOrDefault(Id);

    public bool HasAllMembers { get; internal set; }

    public ConcurrentDictionary<ulong, SocketGuildMember> Members { get; internal set; } = new ConcurrentDictionary<ulong, SocketGuildMember>();
    public ConcurrentDictionary<ulong, Channel> Channels { get; internal set; } = new ConcurrentDictionary<ulong, Channel>();
    public ConcurrentDictionary<ulong, SocketRole> Roles { get; internal set; } = new ConcurrentDictionary<ulong, SocketRole>();

    public SocketGuildMember? GetMember(ulong userId)
    {
        return Members.GetValueOrDefault(userId);
    }

    public SocketRole? GetRole(ulong roleId)
    {
        return Roles.GetValueOrDefault(roleId);
    }

    public Channel? GetChannel(ulong channelId)
    {
        return Channels.GetValueOrDefault(channelId);
    }

    /// <summary>
    /// Permissions for the guild.
    /// </summary>
    public GuildPermissions Permissions { get; internal set; }

    internal SocketGuild(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketGuild Create(FluxerBaseClient client, GuildJson json, SocketGuildMember member)
    {
        SocketGuild data = new SocketGuild(client);
        data.CurrentMember = member;
        data.Members.TryAdd(member.UserId, member);
        data.CurrentMember.Guild = data;
        data.Update(client, json);
        return data;
    }

    internal override void Update(FluxerBaseClient client, GuildJson json)
    {
        base.Update(client, json);
    }

    internal void UpdatePermissions(SocketRole role)
    {
        Permissions = role.Permissions;
    }

    internal void AddOrUpdateMember(FluxerClient client, GuildMemberJson member)
    {
        var mem = SocketGuildMember.Create(client, member);
        mem.Guild = this;
        if (!Members.TryAdd(member.UserId, mem))
            Members[member.UserId].Update(client, member);
    }
}