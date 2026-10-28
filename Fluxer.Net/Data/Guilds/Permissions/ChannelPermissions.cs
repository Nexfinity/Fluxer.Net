namespace Fluxer.Net;

public class ChannelPermissions
{
    public ChannelPermissions(GuildPermission perms)
    {
        RawValue = (ChannelPermission)(ulong)perms;
    }

    public ChannelPermissions(ChannelPermission perms)
    {
        RawValue = perms;
    }

    /// <summary>
    /// Raw permissions flag for the guild.
    /// </summary>
    public ChannelPermission RawValue { get; private set; }

    /// <summary>
    /// Whether every flag in <paramref name="permission"/> is set.
    /// </summary>
    public bool Has(ChannelPermission permission) => (RawValue & permission) == permission;

    /// <summary>
    /// Lists every single permission that is set.
    /// </summary>
    public List<ChannelPermission> ToList()
    {
        List<ChannelPermission> perms = new();
        for (int bit = 0; bit < 64; bit++)
        {
            ChannelPermission flag = (ChannelPermission)(1UL << bit);
            if ((RawValue & flag) != 0)
                perms.Add(flag);
        }

        return perms;
    }

    /// <inheritdoc cref="ChannelPermission.CreateInstantInvite" />
    public bool CreateInstantInvite => RawValue.HasFlag(ChannelPermission.CreateInstantInvite);

    /// <inheritdoc cref="ChannelPermission.ManageChannel" />
    public bool ManageChannel => RawValue.HasFlag(ChannelPermission.ManageChannel);

    /// <inheritdoc cref="ChannelPermission.AddReactions" />
    public bool AddReactions => RawValue.HasFlag(ChannelPermission.AddReactions);

    /// <inheritdoc cref="ChannelPermission.PrioritySpeaker" />
    public bool PrioritySpeaker => RawValue.HasFlag(ChannelPermission.PrioritySpeaker);

    /// <inheritdoc cref="ChannelPermission.Stream" />
    public bool Stream => RawValue.HasFlag(ChannelPermission.Stream);

    /// <inheritdoc cref="ChannelPermission.ViewChannel" />
    public bool ViewChannel => RawValue.HasFlag(ChannelPermission.ViewChannel);

    /// <inheritdoc cref="ChannelPermission.SendMessages" />
    public bool SendMessages => RawValue.HasFlag(ChannelPermission.SendMessages);

    /// <inheritdoc cref="ChannelPermission.SendTTSMessages" />
    public bool SendTTSMessages => RawValue.HasFlag(ChannelPermission.SendTTSMessages);

    /// <inheritdoc cref="ChannelPermission.ManageMessages" />
    public bool ManageMessages => RawValue.HasFlag(ChannelPermission.ManageMessages);

    /// <inheritdoc cref="ChannelPermission.EmbedLinks" />
    public bool EmbedLinks => RawValue.HasFlag(ChannelPermission.EmbedLinks);

    /// <inheritdoc cref="ChannelPermission.AttachFiles" />
    public bool AttachFiles => RawValue.HasFlag(ChannelPermission.AttachFiles);

    /// <inheritdoc cref="ChannelPermission.ReadMessageHistory" />
    public bool ReadMessageHistory => RawValue.HasFlag(ChannelPermission.ReadMessageHistory);

    /// <inheritdoc cref="ChannelPermission.MentionEveryone" />
    public bool MentionEveryone => RawValue.HasFlag(ChannelPermission.MentionEveryone);

    /// <inheritdoc cref="ChannelPermission.UseExternalEmojis" />
    public bool UseExternalEmojis => RawValue.HasFlag(ChannelPermission.UseExternalEmojis);

    /// <inheritdoc cref="ChannelPermission.Connect" />
    public bool Connect => RawValue.HasFlag(ChannelPermission.Connect);

    /// <inheritdoc cref="ChannelPermission.Speak" />
    public bool Speak => RawValue.HasFlag(ChannelPermission.Speak);

    /// <inheritdoc cref="ChannelPermission.MuteMembers" />
    public bool MuteMembers => RawValue.HasFlag(ChannelPermission.MuteMembers);

    /// <inheritdoc cref="ChannelPermission.DeafenMembers" />
    public bool DeafenMembers => RawValue.HasFlag(ChannelPermission.DeafenMembers);

    /// <inheritdoc cref="ChannelPermission.MoveMembers" />
    public bool MoveMembers => RawValue.HasFlag(ChannelPermission.MoveMembers);

    /// <inheritdoc cref="ChannelPermission.UseVAD" />
    public bool UseVAD => RawValue.HasFlag(ChannelPermission.UseVAD);

    /// <inheritdoc cref="ChannelPermission.ManagePermissions" />
    public bool ManagePermissions => RawValue.HasFlag(ChannelPermission.ManagePermissions);

    /// <inheritdoc cref="ChannelPermission.ManageWebhooks" />
    public bool ManageWebhooks => RawValue.HasFlag(ChannelPermission.ManageWebhooks);

    /// <inheritdoc cref="ChannelPermission.ManageThreads" />
    public bool ManageThreads => RawValue.HasFlag(ChannelPermission.ManageThreads);

    /// <inheritdoc cref="ChannelPermission.CreatePublicThreads" />
    public bool CreatePublicThreads => RawValue.HasFlag(ChannelPermission.CreatePublicThreads);

    /// <inheritdoc cref="ChannelPermission.CreatePrivateThreads" />
    public bool CreatePrivateThreads => RawValue.HasFlag(ChannelPermission.CreatePrivateThreads);

    /// <inheritdoc cref="ChannelPermission.UseExternalStickers" />
    public bool UseExternalStickers => RawValue.HasFlag(ChannelPermission.UseExternalStickers);

    /// <inheritdoc cref="ChannelPermission.SendMessagesInThreads" />
    public bool SendMessagesInThreads => RawValue.HasFlag(ChannelPermission.SendMessagesInThreads);

    /// <inheritdoc cref="ChannelPermission.PinMessages" />
    public bool PinMessages => RawValue.HasFlag(ChannelPermission.PinMessages);

    /// <inheritdoc cref="ChannelPermission.BypassSlowmode" />
    public bool BypassSlowmode => RawValue.HasFlag(ChannelPermission.BypassSlowmode);

    /// <inheritdoc cref="ChannelPermission.UpdateRTCRegion" />
    public bool UpdateRTCRegion => RawValue.HasFlag(ChannelPermission.UpdateRTCRegion);

    /// <inheritdoc cref="ChannelPermission.ViewChannelMembers" />
    public bool ViewChannelMembers => RawValue.HasFlag(ChannelPermission.ViewChannelMembers);
}
