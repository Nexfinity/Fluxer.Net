namespace Fluxer.Net;

/// <summary>
/// Which messages notify the user.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Every message notifies the user.
    /// </summary>
    AllMessages = 0,

    /// <summary>
    /// Only mentions notify the user.
    /// </summary>
    MentionsOnly = 1,

    /// <summary>
    /// No messages notify the user.
    /// </summary>
    NoMessages = 2,

    /// <summary>
    /// The setting of the parent is used.
    /// </summary>
    InheritParent = 3,
}
