namespace Fluxer.Net;

/// <summary>
/// The default notification level for new members of a guild.
/// </summary>
public enum GuildDefaultNotifications
{
    /// <summary>
    /// Notify for all messages.
    /// </summary>
    AllMessages = 0,

    /// <summary>
    /// Notify only for mentions.
    /// </summary>
    MentionsOnly = 1,
}
