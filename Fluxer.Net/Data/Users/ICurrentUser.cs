namespace Fluxer.Net;

public interface ICurrentUser : IUser
{
    /// <summary>
    /// Whether the user has staff permissions.
    /// </summary>
    bool IsStaff { get; }

    /// <summary>
    /// Access control list entries for the user.
    /// </summary>
    HashSet<string>? Acls { get; }

    /// <summary>
    /// Special traits assigned to the user account.
    /// </summary>
    HashSet<string>? Traits { get; }

    /// <summary>
    /// The email address associated with the account.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// The phone number associated with the account.
    /// </summary>
    string? Phone { get; }

    /// <summary>
    /// Whether multi-factor authentication is enabled.
    /// </summary>
    bool IsMfaEnabled { get; }

    /// <summary>
    /// Whether the email address has been verified.
    /// </summary>
    bool IsEmailVerified { get; }

    /// <summary>
    /// The type of premium subscription.
    /// </summary>
    PremiumType PremiumType { get; }

    /// <summary>
    /// ISO8601 timestamp of when premium was first activated.
    /// </summary>
    DateTime? PremiumSince { get; }

    /// <summary>
    /// ISO8601 timestamp of when the current premium period ends.
    /// </summary>
    DateTime? PremiumUntil { get; }

    /// <summary>
    /// Whether premium is set to cancel at the end of the billing period.
    /// </summary>
    bool PremiumWillCancel { get; }

    /// <summary>
    /// The billing cycle for the premium subscription.
    /// </summary>
    string? PremiumBillingCycle { get; }

    /// <summary>
    /// The sequence number for lifetime premium subscribers.
    /// </summary>
    int? PremiumLifetimeSequence { get; }

    //
    // TODO ADD MORE PREMIUM STUFF
    //

    DateTime? PasswordLastChangedAt { get; }

    //
    // Required Actions/nsfw
    //

    /// <summary>
    /// Whether the user has ever made a purchase.
    /// </summary>
    bool HasEverPurchased { get; }

    /// <summary>
    /// Whether the current email address is marked as bounced by the mail provider.
    /// </summary>
    bool EmailBounced { get; }

    /// <summary>
    /// The types of authenticators configured for MFA.
    /// </summary>
    HashSet<int>? AuthenticatorTypes { get; }


    //
    // Old properties???
    //

    //string? PasswordHash { get; }

    //string? TotpSecret { get; }

    //string? DateOfBirth { get; }

    //string? Locale { get; }

    //string? StripeSubscriptionId { get; }

    //string? StripeCustomerId { get; }

    //int SuspiciousActivityFlags { get; }

    //DateTime? TermsAgreedAt { get; }

    //DateTime? PrivacyAgreedAt { get; }

    //DateTime? LastActiveAt { get; }

    //string? LastActiveIp { get; }

    //DateTime? TempBannedUntil { get; }

    //DateTime? PendingDeletionAt { get; }

    //int? DeletionReasonCode { get; }

    //string? DeletionPublicReason { get; }

    //string? DeletionAuditLogReason { get; }

    //DateTime? FirstRefundAt { get; }

    //int BetaCodeAllowance { get; }

    //DateTime? BetaCodeLastResetAt { get; }

    //int? GiftInventoryServerSeq { get; }

    //int? GiftInventoryClientSeq { get; }
}
