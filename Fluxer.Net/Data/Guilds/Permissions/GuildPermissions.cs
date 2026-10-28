namespace Fluxer.Net;

public class GuildPermissions
{
    public GuildPermissions(GuildPermission perms)
    {
        RawValue = perms;
    }
    /// <summary>
    /// Raw permissions flag for the guild.
    /// </summary>
    public GuildPermission RawValue { get; private set; }

    /// <summary>
    /// Whether every flag in <paramref name="permission"/> is set.
    /// </summary>
    public bool Has(GuildPermission permission) => (RawValue & permission) == permission;

    /// <summary>
    /// Lists every single permission that is set.
    /// </summary>
    public List<GuildPermission> ToList()
    {
        List<GuildPermission> perms = new();
        for (int bit = 0; bit < 64; bit++)
        {
            GuildPermission flag = (GuildPermission)(1UL << bit);
            if ((RawValue & flag) != 0)
                perms.Add(flag);
        }

        return perms;
    }

    /// <inheritdoc cref="GuildPermission.CreateInstantInvite" />
    public bool CreateInstantInvite => RawValue.HasFlag(GuildPermission.CreateInstantInvite);

    /// <inheritdoc cref="GuildPermission.KickMembers" />
    public bool KickMembers => RawValue.HasFlag(GuildPermission.KickMembers);

    /// <inheritdoc cref="GuildPermission.BanMembers" />
    public bool BanMembers => RawValue.HasFlag(GuildPermission.BanMembers);

    /// <inheritdoc cref="GuildPermission.Administrator" />
    public bool Administrator => RawValue.HasFlag(GuildPermission.Administrator);

    /// <inheritdoc cref="GuildPermission.ManageChannels" />
    public bool ManageChannels => RawValue.HasFlag(GuildPermission.ManageChannels);

    /// <inheritdoc cref="GuildPermission.ManageGuild" />
    public bool ManageGuild => RawValue.HasFlag(GuildPermission.ManageGuild);

    /// <inheritdoc cref="GuildPermission.AddReactions" />
    public bool AddReactions => RawValue.HasFlag(GuildPermission.AddReactions);

    /// <inheritdoc cref="GuildPermission.ViewAuditLog" />
    public bool ViewAuditLog => RawValue.HasFlag(GuildPermission.ViewAuditLog);

    /// <inheritdoc cref="GuildPermission.PrioritySpeaker" />
    public bool PrioritySpeaker => RawValue.HasFlag(GuildPermission.PrioritySpeaker);

    /// <inheritdoc cref="GuildPermission.Stream" />
    public bool Stream => RawValue.HasFlag(GuildPermission.Stream);

    /// <inheritdoc cref="GuildPermission.ViewChannel" />
    public bool ViewChannels => RawValue.HasFlag(GuildPermission.ViewChannel);

    /// <inheritdoc cref="GuildPermission.SendMessages" />
    public bool SendMessages => RawValue.HasFlag(GuildPermission.SendMessages);

    /// <inheritdoc cref="GuildPermission.SendTTSMessages" />
    public bool SendTTSMessages => RawValue.HasFlag(GuildPermission.SendTTSMessages);

    /// <inheritdoc cref="GuildPermission.ManageMessages" />
    public bool ManageMessages => RawValue.HasFlag(GuildPermission.ManageMessages);

    /// <inheritdoc cref="GuildPermission.EmbedLinks" />
    public bool EmbedLinks => RawValue.HasFlag(GuildPermission.EmbedLinks);

    /// <inheritdoc cref="GuildPermission.AttachFiles" />
    public bool AttachFiles => RawValue.HasFlag(GuildPermission.AttachFiles);

    /// <inheritdoc cref="GuildPermission.ReadMessageHistory" />
    public bool ReadMessageHistory => RawValue.HasFlag(GuildPermission.ReadMessageHistory);

    /// <inheritdoc cref="GuildPermission.MentionEveryone" />
    public bool MentionEveryone => RawValue.HasFlag(GuildPermission.MentionEveryone);

    /// <inheritdoc cref="GuildPermission.UseExternalEmojis" />
    public bool UseExternalEmojis => RawValue.HasFlag(GuildPermission.UseExternalEmojis);

    /// <inheritdoc cref="GuildPermission.Connect" />
    public bool Connect => RawValue.HasFlag(GuildPermission.Connect);

    /// <inheritdoc cref="GuildPermission.Speak" />
    public bool Speak => RawValue.HasFlag(GuildPermission.Speak);

    /// <inheritdoc cref="GuildPermission.MuteMembers" />
    public bool MuteMembers => RawValue.HasFlag(GuildPermission.MuteMembers);

    /// <inheritdoc cref="GuildPermission.DeafenMembers" />
    public bool DeafenMembers => RawValue.HasFlag(GuildPermission.DeafenMembers);

    /// <inheritdoc cref="GuildPermission.MoveMembers" />
    public bool MoveMembers => RawValue.HasFlag(GuildPermission.MoveMembers);

    /// <inheritdoc cref="GuildPermission.UseVAD" />
    public bool UseVAD => RawValue.HasFlag(GuildPermission.UseVAD);

    /// <inheritdoc cref="GuildPermission.ChangeNickname" />
    public bool ChangeNickname => RawValue.HasFlag(GuildPermission.ChangeNickname);

    /// <inheritdoc cref="GuildPermission.ManageNicknames" />
    public bool ManageNicknames => RawValue.HasFlag(GuildPermission.ManageNicknames);

    /// <inheritdoc cref="GuildPermission.ManageRoles" />
    public bool ManageRoles => RawValue.HasFlag(GuildPermission.ManageRoles);

    /// <inheritdoc cref="GuildPermission.ManageWebhooks" />
    public bool ManageWebhooks => RawValue.HasFlag(GuildPermission.ManageWebhooks);

    /// <inheritdoc cref="GuildPermission.ManageExpressions" />
    public bool ManageExpressions => RawValue.HasFlag(GuildPermission.ManageExpressions);

    /// <inheritdoc cref="GuildPermission.ManageThreads" />
    public bool ManageThreads => RawValue.HasFlag(GuildPermission.ManageThreads);

    /// <inheritdoc cref="GuildPermission.CreatePublicThreads" />
    public bool CreatePublicThreads => RawValue.HasFlag(GuildPermission.CreatePublicThreads);

    /// <inheritdoc cref="GuildPermission.CreatePrivateThreads" />
    public bool CreatePrivateThreads => RawValue.HasFlag(GuildPermission.CreatePrivateThreads);

    /// <inheritdoc cref="GuildPermission.UseExternalStickers" />
    public bool UseExternalStickers => RawValue.HasFlag(GuildPermission.UseExternalStickers);

    /// <inheritdoc cref="GuildPermission.SendMessagesInThreads" />
    public bool SendMessagesInThreads => RawValue.HasFlag(GuildPermission.SendMessagesInThreads);

    /// <inheritdoc cref="GuildPermission.ModerateMembers" />
    public bool ModerateMembers => RawValue.HasFlag(GuildPermission.ModerateMembers);

    /// <inheritdoc cref="GuildPermission.CreateExpressions" />
    public bool CreateExpressions => RawValue.HasFlag(GuildPermission.CreateExpressions);

    /// <inheritdoc cref="GuildPermission.PinMessages" />
    public bool PinMessages => RawValue.HasFlag(GuildPermission.PinMessages);

    /// <inheritdoc cref="GuildPermission.BypassSlowmode" />
    public bool BypassSlowmode => RawValue.HasFlag(GuildPermission.BypassSlowmode);

    /// <inheritdoc cref="GuildPermission.UpdateRTCRegion" />
    public bool UpdateRTCRegion => RawValue.HasFlag(GuildPermission.UpdateRTCRegion);

    /// <inheritdoc cref="GuildPermission.ViewChannelMembers" />
    public bool ViewChannelMembers => RawValue.HasFlag(GuildPermission.ViewChannelMembers);

    internal static GuildPermissions Resolve(SocketGuildMember member)
    {
        if (member.Id == member.Guild.OwnerId)
            return new GuildPermissions((GuildPermission)ulong.MaxValue);

        GuildPermissions perms = new GuildPermissions(member.Guild.EveryoneRole?.Permissions.RawValue ?? 0);

        foreach (SocketRole i in member.Roles)
        {
            perms.RawValue |= i.Permissions.RawValue;
        }

        if (perms.Administrator)
            return new GuildPermissions((GuildPermission)ulong.MaxValue);

        return perms;
    }
}