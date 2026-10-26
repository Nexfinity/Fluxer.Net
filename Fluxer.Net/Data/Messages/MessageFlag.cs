namespace Fluxer.Net;

/// <summary>
/// Flags that can be applied to messages. Multiple flags can be combined using bitwise OR.
/// </summary>
[Flags]
public enum MessageFlag : ulong
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Flag given to messages that have been published to subscribed
    /// channels (via Channel Following).
    /// </summary>
    Crossposted = 1UL << 0,

    /// <summary>
    /// Flag given to messages that originated from a message in another
    /// channel (via Channel Following).
    /// </summary>
    IsCrosspost = 1UL << 1,

    /// <summary>
    /// Do not include any embeds when serializing this message (link previews hidden).
    /// </summary>
    SuppressEmbeds = 1UL << 2,

    /// <summary>
    ///     Flag given to messages that the source message for this crosspost
    ///     has been deleted (via Channel Following).
    /// </summary>
    SourceMessageDeleted = 1UL << 3,

    /// <summary>
    ///     Flag given to messages has an associated thread, with the same id as the message
    /// </summary>
    HasThread = 1 << 5,

    /// <summary>
    ///     Flag given to messages that failed to mention some roles and add their members to the thread.
    /// </summary>
    FailedToMentionRolesInThread = 1 << 8,

    /// <summary>
    /// Message will not trigger push or desktop notifications.
    /// </summary>
    SuppressNotifications = 1UL << 12,

    /// <summary>
    /// Message is a voice attachment.
    /// </summary>
    VoiceMessage = 1UL << 13,
}
