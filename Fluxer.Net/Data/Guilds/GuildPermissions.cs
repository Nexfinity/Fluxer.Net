namespace Fluxer.Net;

public class GuildPermissions : IGuildPermissions, IChannelPermissions
{
    public GuildPermissions(Permissions perms)
    {
        RawValue = perms;
    }
    /// <summary>
    /// Raw permissions flag for the guild.
    /// </summary>
    public Permissions RawValue { get; internal set; }


    /// <inheritdoc />
    public bool Administrator => RawValue.HasFlag(Permissions.Administrator);

    /// <inheritdoc />
    public bool ViewAuditLog => RawValue.HasFlag(Permissions.ViewAuditLog);

    /// <inheritdoc />
    public bool ManageGuild => RawValue.HasFlag(Permissions.ManageGuild);

    /// <summary>
    /// Create, edit, or delete roles below your highest role. Also allows editing channel permission overwrites.
    /// </summary>
    public bool ManageRoles => RawValue.HasFlag(Permissions.ManageRoles);

    /// <inheritdoc />
    public bool ManageChannels => RawValue.HasFlag(Permissions.ManageChannels);

    /// <inheritdoc />
    public bool KickMembers => RawValue.HasFlag(Permissions.KickMembers);

    /// <inheritdoc />
    public bool BanMembers => RawValue.HasFlag(Permissions.BanMembers);

    /// <inheritdoc />
    public bool CreateInstantInvite => RawValue.HasFlag(Permissions.CreateInstantInvite);

    /// <inheritdoc />
    public bool ChangeNickname => RawValue.HasFlag(Permissions.ChangeNickname);

    /// <inheritdoc />
    public bool ManageNicknames => RawValue.HasFlag(Permissions.ManageNicknames);

    /// <inheritdoc />
    public bool CreateExpressions => RawValue.HasFlag(Permissions.CreateExpressions);

    /// <inheritdoc />
    public bool ManageExpressions => RawValue.HasFlag(Permissions.ManageExpressions);

    /// <inheritdoc />
    public bool ManageWebhooks => RawValue.HasFlag(Permissions.ManageWebhooks);

    /// <inheritdoc />
    public bool ViewChannel => RawValue.HasFlag(Permissions.ViewChannel);

    /// <inheritdoc />
    public bool SendMessages => RawValue.HasFlag(Permissions.SendMessages);

    /// <inheritdoc />
    public bool SendTtsMessages => RawValue.HasFlag(Permissions.SendTtsMessages);

    /// <inheritdoc />
    public bool ManageMessages => RawValue.HasFlag(Permissions.ManageMessages);

    /// <inheritdoc />
    public bool PinMessages => RawValue.HasFlag(Permissions.PinMessages);

    /// <inheritdoc />
    public bool EmbedLinks => RawValue.HasFlag(Permissions.EmbedLinks);

    /// <inheritdoc />
    public bool AttachFiles => RawValue.HasFlag(Permissions.AttachFiles);

    /// <inheritdoc />
    public bool ReadMessageHistory => RawValue.HasFlag(Permissions.ReadMessageHistory);

    /// <inheritdoc />
    public bool MentionEveryone => RawValue.HasFlag(Permissions.MentionEveryone);

    /// <inheritdoc />
    public bool UseExternalEmojis => RawValue.HasFlag(Permissions.UseExternalEmojis);

    /// <inheritdoc />
    public bool UseExternalStickers => RawValue.HasFlag(Permissions.UseExternalStickers);

    /// <inheritdoc />
    public bool AddReactions => RawValue.HasFlag(Permissions.AddReactions);

    /// <inheritdoc />
    public bool BypassSlowmode => RawValue.HasFlag(Permissions.BypassSlowmode);

    /// <inheritdoc />
    public bool ModerateMembers => RawValue.HasFlag(Permissions.ModerateMembers);

    /// <inheritdoc />
    public bool Connect => RawValue.HasFlag(Permissions.Connect);

    /// <inheritdoc />
    public bool Speak => RawValue.HasFlag(Permissions.Speak);

    /// <inheritdoc />
    public bool Stream => RawValue.HasFlag(Permissions.Stream);

    /// <inheritdoc />
    public bool UseVad => RawValue.HasFlag(Permissions.UseVad);

    /// <inheritdoc />
    public bool PrioritySpeaker => RawValue.HasFlag(Permissions.PrioritySpeaker);

    /// <inheritdoc />
    public bool MuteMembers => RawValue.HasFlag(Permissions.MuteMembers);

    /// <inheritdoc />
    public bool DeafenMembers => RawValue.HasFlag(Permissions.DeafenMembers);

    /// <inheritdoc />
    public bool MoveMembers => RawValue.HasFlag(Permissions.MoveMembers);

    /// <inheritdoc />
    public bool UpdateRtcRegion => RawValue.HasFlag(Permissions.UpdateRtcRegion);
}
