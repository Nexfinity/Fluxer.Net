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
    /// No flags set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Fluxer staff member badge.
    /// </summary>
    Staff = 1UL << 0,

    /// <summary>
    /// Community Testing Program (CTP) member badge.
    /// </summary>
    CtpMember = 1UL << 1,

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
    /// User has a premium discriminator (custom tag number).
    /// </summary>
    PremiumDiscriminator = 1UL << 37,

    /// <summary>
    /// User account is disabled (internal flag).
    /// </summary>
    Disabled = 1UL << 38,

    /// <summary>
    /// User has started a session (internal flag).
    /// </summary>
    HasSessionStarted = 1UL << 39,

    /// <summary>
    /// Premium badge is hidden in user profile (user preference).
    /// </summary>
    PremiumBadgeHidden = 1UL << 40,

    /// <summary>
    /// Premium badge is masked/anonymized (internal flag).
    /// </summary>
    PremiumBadgeMasked = 1UL << 41,

    /// <summary>
    /// Premium badge timestamp is hidden (user preference).
    /// </summary>
    PremiumBadgeTimestampHidden = 1UL << 42,

    /// <summary>
    /// Premium badge sequence/tier is hidden (user preference).
    /// </summary>
    PremiumBadgeSequenceHidden = 1UL << 43,

    /// <summary>
    /// Premium perks information is sanitized/filtered (internal flag).
    /// </summary>
    PremiumPerksSanitized = 1UL << 44,

    /// <summary>
    /// Premium purchase is disabled for this user (internal flag).
    /// </summary>
    PremiumPurchaseDisabled = 1UL << 45,

    /// <summary>
    /// Premium enabled via override (admin/staff action, internal flag).
    /// </summary>
    PremiumEnabledOverride = 1UL << 46,

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
    /// User account pending manual verification (internal flag).
    /// </summary>
    PendingManualVerification = 1UL << 50,

    /// <summary>
    /// User has dismissed the premium onboarding flow (user preference).
    /// </summary>
    HasDismissedPremiumOnboarding = 1UL << 51,

    /// <summary>
    /// User has used a mobile client (internal flag)
    /// </summary>
    UsedMobileClient = 1UL << 52,

    /// <summary>
    /// User is an app store reviewer (internal flag)
    /// </summary>
    AppStoreReviewer = 1UL << 53,

    /// <summary>
    /// User DM history has been backfilled (internal flag)
    /// </summary>
    DmHistoryBackfilled = 1UL << 54,

    /// <summary>
    /// User relationships have been indexed (internal flag)
    /// </summary>
    HasRelationshipsIndexed = 1UL << 55,

    /// <summary>
    /// Messages by this author have been backfilled (internal flag)
    /// </summary>
    MessagesByAuthorBackfilled = 1UL << 56,

    /// <summary>
    /// User staff status is hidden from public flags (internal flag)
    /// </summary>
    StaffHidden = 1UL << 57,

    /// <summary>
    /// User's owned bot discriminators have been sanitized (internal flag)
    /// </summary>
    BotSanitized = 1UL << 58,
}
