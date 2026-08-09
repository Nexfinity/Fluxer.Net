namespace Fluxer.Net;

public class GuildPermissions : IGuildPermissions, IChannelPermissions
{
    public GuildPermissions(GuildPermission perms)
    {
        RawValue = perms;
    }
    /// <summary>
    /// Raw permissions flag for the guild.
    /// </summary>
    public GuildPermission RawValue { get; internal set; }


    /// <inheritdoc />
    public bool Administrator => RawValue.HasFlag(GuildPermission.Administrator);

    /// <inheritdoc />
    public bool ViewAuditLog => RawValue.HasFlag(GuildPermission.ViewAuditLog);

    /// <inheritdoc />
    public bool ManageServer => RawValue.HasFlag(GuildPermission.ManageGuild);

    /// <summary>
    /// Create, edit, or delete roles below your highest role. Also allows editing channel permission overwrites.
    /// </summary>
    public bool ManageRoles => RawValue.HasFlag(GuildPermission.ManageRoles);

    /// <inheritdoc />
    public bool ManageChannels => RawValue.HasFlag(GuildPermission.ManageChannels);

    /// <inheritdoc />
    public bool KickMembers => RawValue.HasFlag(GuildPermission.KickMembers);

    /// <inheritdoc />
    public bool BanMembers => RawValue.HasFlag(GuildPermission.BanMembers);

    /// <inheritdoc />
    public bool CreateInstantInvite => RawValue.HasFlag(GuildPermission.CreateInstantInvite);

    /// <inheritdoc />
    public bool ChangeNickname => RawValue.HasFlag(GuildPermission.ChangeNickname);

    /// <inheritdoc />
    public bool ManageNicknames => RawValue.HasFlag(GuildPermission.ManageNicknames);

    /// <inheritdoc />
    public bool CreateExpressions => RawValue.HasFlag(GuildPermission.CreateExpressions);

    /// <inheritdoc />
    public bool ManageExpressions => RawValue.HasFlag(GuildPermission.ManageExpressions);

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
    public bool ModerateMembers => RawValue.HasFlag(GuildPermission.ModerateMembers);

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

    public static GuildPermissions Resolve(SocketGuildMember member)
    {
        GuildPermissions perms = new GuildPermissions(member.Server.EveryoneRole.Permissions.RawValue);

        foreach (var i in member.Roles)
        {
            perms.RawValue |= i.Permissions.RawValue;
        }

        return perms;
    }
}
