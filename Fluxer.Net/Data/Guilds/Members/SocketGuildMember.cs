using System.Collections.Concurrent;

namespace Fluxer.Net;

/// <summary>
/// Cached Guild Member.
/// </summary>
public class SocketGuildMember : GuildMember
{
    /// <summary>
    /// Guild that the member is part of.
    /// </summary>
    public SocketGuild Guild { get; internal set; }

    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; private set; } = new ConcurrentDictionary<string, SocketVoiceState>();

    /// <summary>
    /// List of roles the member is part of.
    /// </summary>
    public IEnumerable<SocketRole> Roles
            => RoleIds.Select(Guild.Roles.GetValueOrDefault).Where(x => x != null);

    public GuildPermissions GuildPermissions => GuildPermissions.Resolve(this);

    /// <summary>
    /// Where the member places in the role hierarchy.
    /// Higher value means higher rank.
    /// </summary>
    public int Hierarchy
    {
        get
        {
            if (Guild.OwnerId == Id)
                return int.MaxValue;

            return Roles.Max(x => x.Position);
        }
    }

    public bool HasPermission(GuildPermission permission)
    {
        if (Id == Guild.OwnerId)
            return true;

        return Roles.Any(r => r.Permissions.Administrator || r.Permissions.RawValue.HasFlag(permission));
    }

    public ChannelPermissions GetPermissions(Channel channel)
    {
        if (Id == Guild.OwnerId)
            return new ChannelPermissions((GuildPermission)ulong.MaxValue);

        GuildPermissions guildPerms = GuildPermissions.Resolve(this);
        if (guildPerms.Administrator)
            return new ChannelPermissions((GuildPermission)ulong.MaxValue);

        ChannelPermission resolvedPermissions = (ChannelPermission)guildPerms.RawValue;

        // "everyone" is a special role that has the guild id as its role id.
        PermissionOverwrite? everyone = channel.PermissionOverwrites.FirstOrDefault(x => x.Id == Guild.Id);
        if (everyone != null)
            resolvedPermissions = (resolvedPermissions & ~everyone.Deny.RawValue) | everyone.Allow.RawValue;

        ChannelPermission deniedPermissions = 0UL, allowedPermissions = 0UL;

        // Check role overwrites.
        foreach (SocketRole r in Roles)
        {
            if (r.Id == Guild.Id)
                continue;

            PermissionOverwrite? role = channel.PermissionOverwrites.FirstOrDefault(x => x.Type == PermissionOverwriteType.Role && x.Id == r.Id);
            if (role != null)
            {
                deniedPermissions |= role.Deny.RawValue;
                allowedPermissions |= role.Allow.RawValue;
            }
        }
        resolvedPermissions = (resolvedPermissions & ~deniedPermissions) | allowedPermissions;

        // Check user overwrite
        PermissionOverwrite? user = channel.PermissionOverwrites.FirstOrDefault(x => x.Type == PermissionOverwriteType.Member && x.Id == Id);
        if (user != null)
            resolvedPermissions = (resolvedPermissions & ~user.Deny.RawValue) | user.Allow.RawValue;


        if (!resolvedPermissions.HasFlag(ChannelPermission.ViewChannel))
            // No view channel permissions = all permissions removed.
            resolvedPermissions = 0;
        else if (!resolvedPermissions.HasFlag(ChannelPermission.SendMessages))
            // These permissions require send messages to work, so we remove them.
            resolvedPermissions &= ~(ChannelPermission.SendTTSMessages | ChannelPermission.MentionEveryone | ChannelPermission.EmbedLinks | ChannelPermission.AttachFiles);

        return new ChannelPermissions(resolvedPermissions);
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
        data.Update(json);
        return data;
    }
}