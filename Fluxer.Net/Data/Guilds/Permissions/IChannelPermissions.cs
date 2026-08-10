namespace Fluxer.Net;

public interface IChannelPermissions
{
    /// <summary>
    /// Create, edit, or delete channels and categories.
    /// </summary>
    bool ManageChannels { get; }

    /// <summary>
    /// Create invites to invite other users to the guild.
    /// </summary>
    bool CreateInstantInvite { get; }

    /// <summary>
    /// Create, edit, or delete webhooks.
    /// </summary>
    bool ManageWebhooks { get; }

    /// <summary>
    /// View this channel.
    /// </summary>
    bool ViewChannel { get; }

    /// <summary>
    /// Send messages in this channel.
    /// </summary>
    bool SendMessages { get; }

    /// <summary>
    /// Send text-to-speech messages.
    /// </summary>
    bool SendTTSMessages { get; }

    /// <summary>
    /// Delete messages from other users.
    /// </summary>
    bool ManageMessages { get; }

    /// <summary>
    /// Pin messages in the channel.
    /// </summary>
    bool PinMessages { get; }

    /// <summary>
    /// Embed links in the channel.
    /// </summary>
    bool EmbedLinks { get; }

    /// <summary>
    /// Upload files in the channel.
    /// </summary>
    bool AttachFiles { get; }

    /// <summary>
    /// Read all messages in the channel.
    /// </summary>
    bool ReadMessageHistory { get; }

    /// <summary>
    /// Mention everyone or any role (even if the role isn't set to be mentionable)
    /// </summary>
    bool MentionEveryone { get; }

    /// <summary>
    /// Use emojis from other guilds.
    /// </summary>
    bool UseExternalEmojis { get; }

    /// <summary>
    /// Use stickers from other guilds.
    /// </summary>
    bool UseExternalStickers { get; }

    /// <summary>
    /// Add new reactions to messages.
    /// </summary>
    bool AddReactions { get; }

    /// <summary>
    /// Ignore per-channel message rate limits..
    /// </summary>
    bool BypassSlowmode { get; }

    /// <summary>
    /// Connect to the voice channel.
    /// </summary>
    bool Connect { get; }

    /// <summary>
    /// Speak in the voice channel.
    /// </summary>
    bool Speak { get; }

    /// <summary>
    /// Allow video and screenshare in the voice channel.
    /// </summary>
    bool Stream { get; }

    /// <summary>
    /// Denied if push to talk is required in the voice channel.
    /// </summary>
    bool UseVad { get; }

    /// <summary>
    /// Prioritize your voice in the voice channel.
    /// </summary>
    bool PrioritySpeaker { get; }

    /// <summary>
    /// Mute users in the voice channel.
    /// </summary>
    bool MuteMembers { get; }

    /// <summary>
    /// Deafen users in the voice channel.
    /// </summary>
    bool DeafenMembers { get; }

    /// <summary>
    /// Drag members between voice channels they can access.
    /// </summary>
    bool MoveMembers { get; }

    /// <summary>
    /// Update the voice region for this voice channel.
    /// </summary>
    bool UpdateRtcRegion { get; }

    /// <summary>
    /// Allows you to view members in the channel.
    /// </summary>
    bool ViewChannelMembers { get; }
}
