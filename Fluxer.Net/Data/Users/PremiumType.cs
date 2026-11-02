namespace Fluxer.Net;

/// <summary>
/// The premium subscription status of a user.
/// </summary>
public enum PremiumType
{
    /// <summary>
    /// No premium subscription.
    /// </summary>
    None = 0,

    /// <summary>
    /// Active premium subscription (recurring).
    /// </summary>
    Subscription = 1,

    /// <summary>
    /// Lifetime premium access (one-time purchase).
    /// </summary>
    Lifetime = 2,
}
