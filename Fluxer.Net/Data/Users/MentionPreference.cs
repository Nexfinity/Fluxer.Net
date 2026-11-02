namespace Fluxer.Net;

/// <summary>
/// The mention preferences of a user.
/// </summary>
public enum MentionPreference
{
    /// <summary>
    /// User has no mention preference.
    /// </summary>
    NoPreference = 0,

    /// <summary>
    /// User wants to be mentioned in replies.
    /// </summary>
    PrefereMention = 1,

    /// <summary>
    /// User does not want to be mentioned in replies.
    /// </summary>
    PrefereNoMention = 2
}
