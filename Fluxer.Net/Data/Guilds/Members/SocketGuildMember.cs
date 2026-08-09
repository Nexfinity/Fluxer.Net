using System.Collections.Concurrent;

namespace Fluxer.Net;

/// <summary>
/// Cached Guild Member.
/// </summary>
public class SocketGuildMember : GuildMember
{
    /// <summary>
    /// Guild that the member is for.
    /// </summary>
    public SocketGuild Server { get; internal set; }

    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; internal set; } = new ConcurrentDictionary<string, SocketVoiceState>();

    public IEnumerable<SocketRole> Roles
            => RoleIds.Select(id => Server.Roles[id]).Where(x => x != null);

    public bool HasPermission(GuildPermission permission)
    {
        if (Id == Server.OwnerId)
            return true;

        foreach (SocketRole r in Roles)
        {
            if (r.Permissions.Administrator)
                return true;

            if (r.Permissions.RawValue.HasFlag(permission))
                return true;
        }

        return false;
    }

    public ChannelPermissions GetPermissions(Channel channel)
    {
        if (Id == Server.OwnerId)
            return new ChannelPermissions((GuildPermission)ulong.MaxValue);

        GuildPermissions guildPerms = GuildPermissions.Resolve(this);
        if (guildPerms.Administrator)
            return new ChannelPermissions((GuildPermission)ulong.MaxValue);

        ulong resolvedPermissions = (ulong)guildPerms.RawValue;

        // Check everyone overwrite
        PermissionOverwrite? everyone = channel.PermissionOverwrites.FirstOrDefault(x => x.Id == Server.Id);
        if (everyone != null)
            resolvedPermissions = (resolvedPermissions & ~(ulong)everyone.Deny.RawValue) | (ulong)everyone.Allow.RawValue;

        ulong deniedPermissions = 0UL, allowedPermissions = 0UL;

        // Check role overwrites
        foreach (var r in Roles)
        {
            if (r.Id == Server.Id)
                continue;

            PermissionOverwrite? role = channel.PermissionOverwrites.FirstOrDefault(x => x.Type == 0 && x.Id == r.Id);
            if (role != null)
            {
                allowedPermissions |= (ulong)role.Allow.RawValue;
                deniedPermissions |= (ulong)role.Deny.RawValue;
            }
        }
        resolvedPermissions = (resolvedPermissions & ~deniedPermissions) | allowedPermissions;

        // Check user overwrite
        PermissionOverwrite? user = channel.PermissionOverwrites.FirstOrDefault(x => x.Type == 1 && x.Id == Id);
        if (user != null)
            resolvedPermissions = (resolvedPermissions & ~(ulong)user.Deny.RawValue) | (ulong)user.Allow.RawValue;


        if (!((ChannelPermission)resolvedPermissions).HasFlag(ChannelPermission.ViewChannel))
        {
            // No view channel permissions all permissions removed.
            resolvedPermissions = 0;
        }
        else if (!((ChannelPermission)resolvedPermissions).HasFlag(ChannelPermission.SendMessages))
        {
            // No send permissions on channel.
            resolvedPermissions &= ~(ulong)ChannelPermission.SendTTSMessages;
            resolvedPermissions &= ~(ulong)ChannelPermission.MentionEveryone;
            resolvedPermissions &= ~(ulong)ChannelPermission.EmbedLinks;
            resolvedPermissions &= ~(ulong)ChannelPermission.AttachFiles;
        }

        return new ChannelPermissions((GuildPermission)resolvedPermissions);
    }

    internal SocketGuildMember(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketGuildMember object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static new SocketGuildMember Create(FluxerBaseClient client, GuildMemberJson json)
    {
        SocketGuildMember data = new SocketGuildMember(client);
        data.Update(client, json);
        return data;
    }

    internal override void Update(FluxerBaseClient client, GuildMemberJson json)
    {
        base.Update(client, json);
    }
}