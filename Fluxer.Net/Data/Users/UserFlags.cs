namespace Fluxer.Net;

/// <summary>
/// Represents user account flags and badges in Fluxer. Multiple flags can be combined using bitwise OR.
/// </summary>
/// <remarks>
/// User flags indicate special properties, badges, or statuses associated with a user account.
/// These are typically set by the Fluxer platform and not directly modifiable by users.
/// </remarks>
[Flags]
public enum UserFlags : ulong
{
    /// <summary>
    /// Fluxer staff member badge.
    /// </summary>
    Staff = 1UL << 0,

    /// <summary>
    /// Fluxer partner badge.
    /// </summary>
    Partner = 1UL << 2,

    /// <summary>
    /// Bug hunter badge (found and reported bugs).
    /// </summary>
    BugHunter = 1UL << 3,

    /// <summary>
    /// Bot accepts friend requests from users.
    /// </summary>
    FriendlyBot = 1UL << 4,

    /// <summary>
    /// Bot requires manual approval for friend requests.
    /// </summary>
    FriendlyBotManualApproval = 1UL << 5,

    /// <summary>
    /// User flagged as a spammer.
    /// </summary>
    Spammer = 1UL << 6,

    /// <summary>
    /// User has a higher global rate limit (internal flag).
    /// </summary>
    HighGlobalRateLimit = 1UL << 33,

    /// <summary>
    /// User account has been deleted (internal flag).
    /// </summary>
    Deleted = 1UL << 34,

    /// <summary>
    /// Account disabled due to suspicious activity (internal flag).
    /// </summary>
    DisabledSuspiciousActivity = 1UL << 35,

    /// <summary>
    /// User self-deleted their account (internal flag).
    /// </summary>
    SelfDeleted = 1UL << 36,

    /// <summary>
    /// User account is disabled (internal flag).
    /// </summary>
    Disabled = 1UL << 38,

    /// <summary>
    /// User has started a session (internal flag).
    /// </summary>
    HasSessionStarted = 1UL << 39,

    /// <summary>
    /// User can bypass rate limits (staff/special accounts, internal flag).
    /// </summary>
    RateLimitBypass = 1UL << 47,

    /// <summary>
    /// User is banned from submitting reports (internal flag).
    /// </summary>
    ReportBanned = 1UL << 48,

    /// <summary>
    /// User is verified as not underage (internal flag).
    /// </summary>
    VerifiedNotUnderage = 1UL << 49,

    /// <summary>
    /// User has dismissed the premium onboarding flow (user preference).
    /// </summary>
    HasDismissedPremiumOnboarding = 1UL << 51,

    /// <summary>
    /// User is an app store reviewer (internal flag)
    /// </summary>
    AppStoreReviewer = 1UL << 53,

    /// <summary>
    /// User staff status is hidden from public flags (internal flag)
    /// </summary>
    StaffHidden = 1UL << 57,

    /// <summary>
    /// User has verified they are an adult (internal flag)
    /// </summary>
    AgeVerifiedAdult = 1UL << 60,

    /// <summary>
    /// Force phone verification for this user (internal flag)
    /// </summary>
    ForcePhoneVerification = 1UL << 61,

    /// <summary>
    /// User is exempt from automatic flagging (internal flag)
    /// </summary>
    NotSuspicious = 1UL << 61,
}
