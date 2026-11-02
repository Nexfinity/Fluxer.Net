namespace Fluxer.Net;

/// <summary>
/// The flags of a message, combined as a bitfield.
/// </summary>
[Flags]
public enum MessageFlag : ulong
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The message has been published to channels that follow its channel.
    /// </summary>
    Crossposted = 1UL << 0,

    /// <summary>
    /// The message is a crosspost of a message from a followed channel.
    /// </summary>
    IsCrosspost = 1UL << 1,

    /// <summary>
    /// Embeds are not shown for the message, such as link previews.
    /// </summary>
    SuppressEmbeds = 1UL << 2,

    /// <summary>
    /// The source message of this crosspost has been deleted.
    /// </summary>
    SourceMessageDeleted = 1UL << 3,

    /// <summary>
    /// The message has a thread with the same id as the message.
    /// </summary>
    HasThread = 1 << 5,

    /// <summary>
    /// The message failed to mention some roles and add their members to the thread.
    /// </summary>
    FailedToMentionRolesInThread = 1 << 8,

    /// <summary>
    /// The message does not trigger push or desktop notifications.
    /// </summary>
    SuppressNotifications = 1UL << 12,

    /// <summary>
    /// The message is a voice message.
    /// </summary>
    VoiceMessage = 1UL << 13,
}
