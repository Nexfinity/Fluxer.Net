namespace Fluxer.Net;

/// <summary>
/// Whether replies to a user should mention them by default.
/// </summary>
public enum MentionReplyPreference
{
    /// <summary>
    /// Respect the sender's choice on each reply. On a guild member, the user-level preference applies.
    /// </summary>
    NoPreference = 0,

    /// <summary>
    /// Replies mention the user by default, and the sender is warned when turning the mention off.
    /// </summary>
    PreferMention = 1,

    /// <summary>
    /// Replies leave out the mention by default, and the sender is warned when turning it on.
    /// </summary>
    PreferNoMention = 2
}
