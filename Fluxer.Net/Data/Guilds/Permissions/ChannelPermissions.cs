namespace Fluxer.Net;

public class ChannelPermissions : IChannelPermissions
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

    /// <inheritdoc />
    public bool CreateInstantInvite => RawValue.HasFlag(ChannelPermission.CreateInstantInvite);

    /// <inheritdoc />
    public bool ManageChannel => RawValue.HasFlag(ChannelPermission.ManageChannel);

    /// <inheritdoc />
    public bool AddReactions => RawValue.HasFlag(ChannelPermission.AddReactions);

    /// <inheritdoc />
    public bool PrioritySpeaker => RawValue.HasFlag(ChannelPermission.PrioritySpeaker);

    /// <inheritdoc />
    public bool Stream => RawValue.HasFlag(ChannelPermission.Stream);

    /// <inheritdoc />
    public bool ViewChannel => RawValue.HasFlag(ChannelPermission.ViewChannel);

    /// <inheritdoc />
    public bool SendMessages => RawValue.HasFlag(ChannelPermission.SendMessages);

    /// <inheritdoc />
    public bool SendTTSMessages => RawValue.HasFlag(ChannelPermission.SendTTSMessages);

    /// <inheritdoc />
    public bool ManageMessages => RawValue.HasFlag(ChannelPermission.ManageMessages);

    /// <inheritdoc />
    public bool EmbedLinks => RawValue.HasFlag(ChannelPermission.EmbedLinks);

    /// <inheritdoc />
    public bool AttachFiles => RawValue.HasFlag(ChannelPermission.AttachFiles);

    /// <inheritdoc />
    public bool ReadMessageHistory => RawValue.HasFlag(ChannelPermission.ReadMessageHistory);

    /// <inheritdoc />
    public bool MentionEveryone => RawValue.HasFlag(ChannelPermission.MentionEveryone);

    /// <inheritdoc />
    public bool UseExternalEmojis => RawValue.HasFlag(ChannelPermission.UseExternalEmojis);

    /// <inheritdoc />
    public bool Connect => RawValue.HasFlag(ChannelPermission.Connect);

    /// <inheritdoc />
    public bool Speak => RawValue.HasFlag(ChannelPermission.Speak);

    /// <inheritdoc />
    public bool MuteMembers => RawValue.HasFlag(ChannelPermission.MuteMembers);

    /// <inheritdoc />
    public bool DeafenMembers => RawValue.HasFlag(ChannelPermission.DeafenMembers);

    /// <inheritdoc />
    public bool MoveMembers => RawValue.HasFlag(ChannelPermission.MoveMembers);

    /// <inheritdoc />
    public bool UseVad => RawValue.HasFlag(ChannelPermission.UseVad);

    /// <inheritdoc />
    public bool ManagePermissions => RawValue.HasFlag(ChannelPermission.ManagePermissions);

    /// <inheritdoc />
    public bool ManageWebhooks => RawValue.HasFlag(ChannelPermission.ManageWebhooks);

    /// <inheritdoc />
    public bool ManageThreads => RawValue.HasFlag(ChannelPermission.ManageThreads);

    /// <inheritdoc />
    public bool CreatePublicThreads => RawValue.HasFlag(ChannelPermission.CreatePublicThreads);

    /// <inheritdoc />
    public bool CreatePrivateThreads => RawValue.HasFlag(ChannelPermission.CreatePrivateThreads);

    /// <inheritdoc />
    public bool UseExternalStickers => RawValue.HasFlag(ChannelPermission.UseExternalStickers);

    /// <inheritdoc />
    public bool SendMessagesInThreads => RawValue.HasFlag(ChannelPermission.SendMessagesInThreads);

    /// <inheritdoc />
    public bool PinMessages => RawValue.HasFlag(ChannelPermission.PinMessages);

    /// <inheritdoc />
    public bool BypassSlowmode => RawValue.HasFlag(ChannelPermission.BypassSlowmode);

    /// <inheritdoc />
    public bool UpdateRtcRegion => RawValue.HasFlag(ChannelPermission.UpdateRtcRegion);

    /// <inheritdoc />
    public bool ViewChannelMembers => RawValue.HasFlag(ChannelPermission.ViewChannelMembers);

}
