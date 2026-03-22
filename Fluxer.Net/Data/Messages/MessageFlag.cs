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
    /// Do not include any embeds when serializing this message (link previews hidden).
    /// </summary>
    SuppressEmbeds = 1UL << 2,

    /// <summary>
    /// Message will not trigger push or desktop notifications.
    /// </summary>
    SuppressNotifications = 1UL << 12,

    /// <summary>
    /// Message is a voice attachment.
    /// </summary>
    VoiceMessage = 1UL << 13,

    CompactAttachments = 1UL << 17
}
