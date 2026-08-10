namespace Fluxer.Net;

/// <summary>
/// Represents guild and channel permissions in Fluxer. Multiple permissions can be combined using bitwise OR.
/// </summary>
/// <remarks>
/// Permissions are stored as a 64-bit unsigned integer where each bit represents a specific permission.
/// Use bitwise operations to check, add, or remove permissions.
/// </remarks>
[Flags]
public enum GuildPermission : ulong
{
    /// <summary>
    /// No permissions granted.
    /// </summary>
    None = 0,

    /// <summary>
    /// Allows creation of instant invites to the guild or channel.
    /// </summary>
    CreateInstantInvite = 1UL << 0,

    /// <summary>
    /// Allows kicking members from the guild.
    /// </summary>
    KickMembers = 1UL << 1,

    /// <summary>
    /// Allows banning members from the guild.
    /// </summary>
    BanMembers = 1UL << 2,

    /// <summary>
    /// Grants all permissions and bypasses channel permission overwrites.
    /// </summary>
    Administrator = 1UL << 3,

    /// <summary>
    /// Allows management and editing of channels.
    /// </summary>
    ManageChannels = 1UL << 4,

    /// <summary>
    /// Allows management and editing of the guild.
    /// </summary>
    ManageGuild = 1UL << 5,

    /// <summary>
    /// Allows adding reactions to messages.
    /// </summary>
    AddReactions = 1UL << 6,

    /// <summary>
    /// Allows viewing the guild audit log.
    /// </summary>
    ViewAuditLog = 1UL << 7,

    /// <summary>
    /// Allows using priority speaker in voice channels.
    /// </summary>
    PrioritySpeaker = 1UL << 8,

    /// <summary>
    /// Allows streaming video in voice channels.
    /// </summary>
    Stream = 1UL << 9,

    /// <summary>
    /// Allows viewing channels (includes reading messages in text channels and joining voice channels).
    /// </summary>
    ViewChannel = 1UL << 10,

    /// <summary>
    /// Allows sending messages in text channels.
    /// </summary>
    SendMessages = 1UL << 11,

    /// <summary>
    /// Allows sending text-to-speech messages.
    /// </summary>
    SendTTSMessages = 1UL << 12,

    /// <summary>
    /// Allows deleting and pinning messages from other users.
    /// </summary>
    ManageMessages = 1UL << 13,

    /// <summary>
    /// Allows links sent by the user to auto-embed.
    /// </summary>
    EmbedLinks = 1UL << 14,

    /// <summary>
    /// Allows uploading files and media to messages.
    /// </summary>
    AttachFiles = 1UL << 15,

    /// <summary>
    /// Allows reading message history in channels.
    /// </summary>
    ReadMessageHistory = 1UL << 16,

    /// <summary>
    /// Allows using @everyone and @here mentions, and mentioning all roles.
    /// </summary>
    MentionEveryone = 1UL << 17,

    /// <summary>
    /// Allows using emojis from other guilds.
    /// </summary>
    UseExternalEmojis = 1UL << 18,

    /// <summary>
    /// Allows viewing guild insights and analytics.
    /// </summary>
    ViewGuildInsights = 1UL << 19,

    /// <summary>
    /// Allows joining voice channels.
    /// </summary>
    Connect = 1UL << 20,

    /// <summary>
    /// Allows speaking in voice channels.
    /// </summary>
    Speak = 1UL << 21,

    /// <summary>
    /// Allows muting members in voice channels.
    /// </summary>
    MuteMembers = 1UL << 22,

    /// <summary>
    /// Allows deafening members in voice channels.
    /// </summary>
    DeafenMembers = 1UL << 23,

    /// <summary>
    /// Allows moving members between voice channels.
    /// </summary>
    MoveMembers = 1UL << 24,

    /// <summary>
    /// Allows using voice activity detection (no push-to-talk required).
    /// </summary>
    UseVad = 1UL << 25,

    /// <summary>
    /// Allows changing own nickname.
    /// </summary>
    ChangeNickname = 1UL << 26,

    /// <summary>
    /// Allows managing nicknames of other members.
    /// </summary>
    ManageNicknames = 1UL << 27,

    /// <summary>
    /// Allows management and editing of roles.
    /// </summary>
    ManageRoles = 1UL << 28,

    /// <summary>
    /// Allows management and editing of webhooks.
    /// </summary>
    ManageWebhooks = 1UL << 29,

    /// <summary>
    /// Allows management and editing of emojis, stickers, and soundboard sounds.
    /// </summary>
    ManageExpressions = 1UL << 30,

    /// <summary>
    /// Allows using application commands (slash commands, context menu commands).
    /// </summary>
    UseApplicationCommands = 1UL << 31,

    /// <summary>
    /// Allows requesting to speak in stage channels.
    /// </summary>
    RequestToSpeak = 1UL << 32,

    /// <summary>
    /// Allows managing guild events.
    /// </summary>
    ManageEvents = 1UL << 33,

    /// <summary>
    /// Allows management of threads (archiving, locking, deleting).
    /// </summary>
    ManageThreads = 1UL << 34,

    /// <summary>
    /// Allows creating public threads.
    /// </summary>
    CreatePublicThreads = 1UL << 35,

    /// <summary>
    /// Allows creating private threads.
    /// </summary>
    CreatePrivateThreads = 1UL << 36,

    /// <summary>
    /// Allows using stickers from other guilds.
    /// </summary>
    UseExternalStickers = 1UL << 37,

    /// <summary>
    /// Allows sending messages in threads.
    /// </summary>
    SendMessagesInThreads = 1UL << 38,

    /// <summary>
    /// Allows using embedded activities (games, activities) in voice channels.
    /// </summary>
    UseEmbeddedActivities = 1UL << 39,

    /// <summary>
    /// Allows timing out members (preventing them from sending messages/joining voice).
    /// </summary>
    ModerateMembers = 1UL << 40,

    /// <summary>
    /// Allows viewing monetization analytics for creators.
    /// </summary>
    ViewCreatorMonetizationAnalytics = 1UL << 41,

    /// <summary>
    /// Allows using the soundboard in voice channels.
    /// </summary>
    UseSoundboard = 1UL << 42,

    /// <summary>
    /// Allows creating custom emojis, stickers, and soundboard sounds.
    /// </summary>
    CreateExpressions = 1UL << 43,

    /// <summary>
    /// Allows creating guild events.
    /// </summary>
    CreateEvents = 1UL << 44,

    /// <summary>
    /// Allows using soundboard sounds from other guilds.
    /// </summary>
    UseExternalSounds = 1UL << 45,

    /// <summary>
    /// Allows sending voice messages in text channels.
    /// </summary>
    SendVoiceMessages = 1UL << 46,

    /// <summary>
    /// Allows setting a custom status for voice channels.
    /// </summary>
    SetVoiceChannelStatus = 1UL << 48,

    /// <summary>
    /// Allows sending polls in text channels.
    /// </summary>
    SendPolls = 1UL << 49,

    /// <summary>
    /// Allows using external applications in channels.
    /// </summary>
    UseExternalApps = 1UL << 50,

    /// <summary>
    /// Allows pinning messages in channels.
    /// </summary>
    PinMessages = 1UL << 51,

    /// <summary>
    /// Allows bypassing channel slowmode restrictions.
    /// </summary>
    BypassSlowmode = 1UL << 52,

    /// <summary>
    /// Allows updating the RTC (voice) region for a channel.
    /// </summary>
    UpdateRtcRegion = 1UL << 53,

    /// <summary>
    /// Allows you to view members in the channel.
    /// </summary>
    ViewChannelMembers = 1UL << 54,
}