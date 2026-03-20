namespace Fluxer.Net;

public interface IUser
{
    ulong Id { get; }

    string Username { get; }

    int Discriminator { get; }

    bool IsBot { get; }

    bool IsSystem { get; }

    string? Email { get; }

    bool EmailVerified { get; }

    bool EmailBounced { get; }

    string? Phone { get; }

    string? PasswordHash { get; }

    DateTime? PasswordLastChangedAt { get; }

    string? TotpSecret { get; }

    HashSet<int>? AuthenticatorTypes { get; }

    string? AvatarHash { get; }

    string? BannerHash { get; }

    string? Bio { get; }

    string? Pronouns { get; }

    int? AccentColor { get; }

    string? DateOfBirth { get; }

    string? Locale { get; }

    ulong Flags { get; }

    int? PremiumType { get; }

    DateTime? PremiumSince { get; }

    DateTime? PremiumUntil { get; }

    bool PremiumWillCancel { get; }

    string? PremiumBillingCycle { get; }

    int? PremiumLifetimeSequence { get; }

    string? StripeSubscriptionId { get; }

    string? StripeCustomerId { get; }

    bool HasEverPurchased { get; }

    int SuspiciousActivityFlags { get; }

    DateTime? TermsAgreedAt { get; }

    DateTime? PrivacyAgreedAt { get; }

    DateTime? LastActiveAt { get; }

    string? LastActiveIp { get; }

    DateTime? TempBannedUntil { get; }

    DateTime? PendingDeletionAt { get; }

    int? DeletionReasonCode { get; }

    string? DeletionPublicReason { get; }

    string? DeletionAuditLogReason { get; }

    HashSet<string>? Acls { get; }

    DateTime? FirstRefundAt { get; }

    int BetaCodeAllowance { get; }

    DateTime? BetaCodeLastResetAt { get; }

    int? GiftInventoryServerSeq { get; }

    int? GiftInventoryClientSeq { get; }
}
