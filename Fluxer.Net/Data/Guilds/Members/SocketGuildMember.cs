using System.Collections.Concurrent;

namespace Fluxer.Net;

/// <summary>
/// A guild member cached from gateway events.
/// </summary>
public class SocketGuildMember : GuildMember
{
    /// <summary>
    /// Guild that the member is part of.
    /// </summary>
    public SocketGuild Guild { get; internal set; }

    /// <summary>
    /// The voice states of the member, keyed by connection ID or session ID.
    /// </summary>
    public ConcurrentDictionary<string, SocketVoiceState> VoiceStates { get; private set; } = new ConcurrentDictionary<string, SocketVoiceState>();

    internal override GuildMember Snapshot()
    {
        SocketGuildMember copy = (SocketGuildMember)base.Snapshot();
        copy.VoiceStates = new ConcurrentDictionary<string, SocketVoiceState>(VoiceStates);
        return copy;
    }

    /// <summary>
    /// The cached roles the member has.
    /// </summary>
    public IEnumerable<SocketRole> Roles
            => RoleIds.Select(Guild.Roles.GetValueOrDefault).Where(x => x != null);

    /// <summary>
    /// The guild permissions of the member, from the @everyone role and the member's roles. All permissions when the member is the owner or an administrator.
    /// </summary>
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

            return Roles.Select(x => x.Position).DefaultIfEmpty(0).Max();
        }
    }

    /// <summary>
    /// Whether the member's resolved <see cref="GuildPermissions"/> include the permission.
    /// </summary>
    /// <param name="permission">The permission to check.</param>
    public bool HasPermission(GuildPermission permission) => GuildPermissions.Resolve(this).Has(permission);

    /// <summary>
    /// Gets the member's permissions in a channel, with the channel's permission overwrites applied.
    /// </summary>
    /// <param name="channel">The channel to resolve the permissions for.</param>
    public ChannelPermissions GetPermissions(Channel channel)
    {
        if (Id == Guild.OwnerId)
            return new ChannelPermissions((GuildPermission)ulong.MaxValue);

        GuildPermissions guildPerms = GuildPermissions.Resolve(this);
        if (guildPerms.Administrator)
            return new ChannelPermissions((GuildPermission)ulong.MaxValue);

        ChannelPermission resolvedPermissions = (ChannelPermission)guildPerms.RawValue;
        IEnumerable<PermissionOverwrite> overwrites = channel.PermissionOverwrites ?? [];

        // "everyone" is a special role that has the guild id as its role id.
        PermissionOverwrite? everyone = overwrites.FirstOrDefault(x => x.Id == Guild.Id);
        if (everyone != null)
            resolvedPermissions = (resolvedPermissions & ~everyone.Deny.RawValue) | everyone.Allow.RawValue;

        ChannelPermission deniedPermissions = 0UL, allowedPermissions = 0UL;

        // Check role overwrites.
        foreach (SocketRole r in Roles)
        {
            if (r.Id == Guild.Id)
                continue;

            PermissionOverwrite? role = overwrites.FirstOrDefault(x => x.Type == PermissionOverwriteType.Role && x.Id == r.Id);
            if (role != null)
            {
                deniedPermissions |= role.Deny.RawValue;
                allowedPermissions |= role.Allow.RawValue;
            }
        }
        resolvedPermissions = (resolvedPermissions & ~deniedPermissions) | allowedPermissions;

        // Check user overwrite
        PermissionOverwrite? user = overwrites.FirstOrDefault(x => x.Type == PermissionOverwriteType.Member && x.Id == Id);
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
    /// Creates a cached member from its JSON representation.
    /// </summary>
    /// <returns><see cref="SocketGuildMember"/></returns>
    public static new SocketGuildMember Create(FluxerBaseClient client, GuildMemberJson json)
    {
        SocketGuildMember data = new SocketGuildMember(client);
        data.Update(json);
        return data;
    }
}