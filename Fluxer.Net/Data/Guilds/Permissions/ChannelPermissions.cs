namespace Fluxer.Net;

public class ChannelPermissions : IChannelPermissions
{
    public ChannelPermissions(GuildPermission perms)
    {
        RawValue = (ChannelPermission)(ulong)perms;
    }
    /// <summary>
    /// Raw permissions flag for the guild.
    /// </summary>
    public ChannelPermission RawValue { get; internal set; }

    /// <summary>
    /// Edit overwrites for roles and members in this channel.
    /// </summary>
    public bool ManagePermissions => RawValue.HasFlag(GuildPermission.ManageRoles);

    /// <inheritdoc />
    public bool ManageChannels => RawValue.HasFlag(GuildPermission.ManageChannels);

    /// <inheritdoc />
    public bool CreateInstantInvite => RawValue.HasFlag(GuildPermission.CreateInstantInvite);

    /// <inheritdoc />
    public bool ManageWebhooks => RawValue.HasFlag(GuildPermission.ManageWebhooks);

    /// <inheritdoc />
    public bool ViewChannel => RawValue.HasFlag(GuildPermission.ViewChannel);

    /// <inheritdoc />
    public bool SendMessages => RawValue.HasFlag(GuildPermission.SendMessages);

    /// <inheritdoc />
    public bool SendTtsMessages => RawValue.HasFlag(GuildPermission.SendTtsMessages);

    /// <inheritdoc />
    public bool ManageMessages => RawValue.HasFlag(GuildPermission.ManageMessages);

    /// <inheritdoc />
    public bool PinMessages => RawValue.HasFlag(GuildPermission.PinMessages);

    /// <inheritdoc />
    public bool EmbedLinks => RawValue.HasFlag(GuildPermission.EmbedLinks);

    /// <inheritdoc />
    public bool AttachFiles => RawValue.HasFlag(GuildPermission.AttachFiles);

    /// <inheritdoc />
    public bool ReadMessageHistory => RawValue.HasFlag(GuildPermission.ReadMessageHistory);

    /// <inheritdoc />
    public bool MentionEveryone => RawValue.HasFlag(GuildPermission.MentionEveryone);

    /// <inheritdoc />
    public bool UseExternalEmojis => RawValue.HasFlag(GuildPermission.UseExternalEmojis);

    /// <inheritdoc />
    public bool UseExternalStickers => RawValue.HasFlag(GuildPermission.UseExternalStickers);

    /// <inheritdoc />
    public bool AddReactions => RawValue.HasFlag(GuildPermission.AddReactions);

    /// <inheritdoc />
    public bool BypassSlowmode => RawValue.HasFlag(GuildPermission.BypassSlowmode);

    /// <inheritdoc />
    public bool Connect => RawValue.HasFlag(GuildPermission.Connect);

    /// <inheritdoc />
    public bool Speak => RawValue.HasFlag(GuildPermission.Speak);

    /// <inheritdoc />
    public bool Stream => RawValue.HasFlag(GuildPermission.Stream);

    /// <inheritdoc />
    public bool UseVad => RawValue.HasFlag(GuildPermission.UseVad);

    /// <inheritdoc />
    public bool PrioritySpeaker => RawValue.HasFlag(GuildPermission.PrioritySpeaker);

    /// <inheritdoc />
    public bool MuteMembers => RawValue.HasFlag(GuildPermission.MuteMembers);

    /// <inheritdoc />
    public bool DeafenMembers => RawValue.HasFlag(GuildPermission.DeafenMembers);

    /// <inheritdoc />
    public bool MoveMembers => RawValue.HasFlag(GuildPermission.MoveMembers);

    /// <inheritdoc />
    public bool UpdateRtcRegion => RawValue.HasFlag(GuildPermission.UpdateRtcRegion);

    /// <inheritdoc />
    public bool ViewChannelMembers => RawValue.HasFlag(GuildPermission.ViewChannelMembers);

    internal static ulong ResolveChannel(SocketGuild guild, SocketGuildMember member, Channel channel)
    {
        ulong resolvedPermissions = 0;

        // Max permissions
        ulong mask = ulong.MaxValue;

        // Current guild permissions.
        foreach (SocketRole r in member.Roles)
        {
            resolvedPermissions |= (ulong)r.Permissions.RawValue;
        }

        // Everyone overwrite.
        PermissionOverwrite? everyoneOverwrite = channel.PermissionOverwrites.FirstOrDefault(x => x.Id == guild.EveryoneRole.Id);
        if (everyoneOverwrite != null)
            resolvedPermissions = (resolvedPermissions & ~(ulong)everyoneOverwrite.Deny.RawValue) | (ulong)everyoneOverwrite.Allow.RawValue;

        ulong deniedPermissions = 0, allowedPermissions = 0;


        return resolvedPermissions;
    }
}
