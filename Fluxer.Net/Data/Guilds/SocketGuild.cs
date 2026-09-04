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
    public SocketGuildMember CurrentMember { get; private set; }

    public SocketRole EveryoneRole => Roles.GetValueOrDefault(Id);

    public bool HasAllMembers { get; internal set; }

    public ConcurrentDictionary<ulong, SocketGuildMember> Members { get; private set; } = new ConcurrentDictionary<ulong, SocketGuildMember>();
    public ConcurrentDictionary<ulong, Channel> Channels { get; private set; } = new ConcurrentDictionary<ulong, Channel>();
    public ConcurrentDictionary<ulong, SocketRole> Roles { get; private set; } = new ConcurrentDictionary<ulong, SocketRole>();

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

    /// <summary>
    /// Create a SocketGuild object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static SocketGuild Create(FluxerBaseClient client, GuildJson json, SocketGuildMember currentMember)
    {
        SocketGuild data = new SocketGuild(client)
        {
            CurrentMember = currentMember
        };
        data.Members.TryAdd(currentMember.Id, currentMember);
        data.CurrentMember.Guild = data;

        // Null count data on socket guild.
        data.OnlineCount = null;
        data.MemberCount = null;
        data.Update(json);
        return data;
    }

    internal void UpdatePermissions(SocketRole role)
    {
        Permissions = role.Permissions;
    }

    internal SocketGuildMember AddOrUpdateMember(GuildMemberJson json)
    {
        if (Members.TryGetValue(json.Id, out var member))
        {
            member.Update(json);
            return member;
        }
        else
        {
            member = SocketGuildMember.Create(Client, json);
            member.Guild = this;
            Members.TryAdd(member.Id, member);
            return member;
        }
    }
}