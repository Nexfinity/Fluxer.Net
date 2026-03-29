namespace Fluxer.Net;

[Flags]
public enum GuildOperations : ulong
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Push notifications for this guild.
    /// </summary>
    PushNotifications = 1UL << 0,

    /// <summary>
    /// @everyone mentions in this guild.
    /// </summary>
    EveryoneMentions = 1UL << 1,

    /// <summary>
    /// Typing indicator events.
    /// </summary>
    TypingEvents = 1UL << 2,

    /// <summary>
    /// Creation of instant invites.
    /// </summary>
    InstantInvites = 1UL << 3,

    /// <summary>
    /// Sending messages in the guild.
    /// </summary>
    SendMessages = 1UL << 4,

    /// <summary>
    /// Adding reactions to messages.
    /// </summary>
    Reactions = 1UL << 5,

    /// <summary>
    /// Member list update events.
    /// </summary>
    MemberListUpdates = 1UL << 6,
}
