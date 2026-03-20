using Newtonsoft.Json;

namespace Fluxer.Net;

public class UserJson : IUser
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("discriminator")]
    public int Discriminator { get; set; }

    [JsonProperty("bot")]
    public bool IsBot { get; set; }

    [JsonProperty("system")]
    public bool IsSystem { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("email_verified")]
    public bool EmailVerified { get; set; }

    [JsonProperty("email_bounced")]
    public bool EmailBounced { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("password_hash")]
    public string? PasswordHash { get; set; }

    [JsonProperty("password_last_changed_at")]
    public DateTime? PasswordLastChangedAt { get; set; }

    [JsonProperty("totp_secret")]
    public string? TotpSecret { get; set; }

    [JsonProperty("authenticator_types")]
    public HashSet<int>? AuthenticatorTypes { get; set; }

    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    [JsonProperty("bio")]
    public string? Bio { get; set; }

    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    [JsonProperty("date_of_birth")]
    public string? DateOfBirth { get; set; }

    [JsonProperty("locale")]
    public string? Locale { get; set; }

    [JsonProperty("flags")]
    public ulong Flags { get; set; }

    [JsonProperty("premium_type")]
    public int? PremiumType { get; set; }

    [JsonProperty("premium_since")]
    public DateTime? PremiumSince { get; set; }

    [JsonProperty("premium_until")]
    public DateTime? PremiumUntil { get; set; }

    [JsonProperty("premium_will_cancel")]
    public bool PremiumWillCancel { get; set; }

    [JsonProperty("premium_billing_cycle")]
    public string? PremiumBillingCycle { get; set; }

    [JsonProperty("premium_lifetime_sequence")]
    public int? PremiumLifetimeSequence { get; set; }

    [JsonProperty("stripe_subscription_id")]
    public string? StripeSubscriptionId { get; set; }

    [JsonProperty("stripe_customer_id")]
    public string? StripeCustomerId { get; set; }

    [JsonProperty("has_ever_purchased")]
    public bool HasEverPurchased { get; set; }

    [JsonProperty("suspicious_activity_flags")]
    public int SuspiciousActivityFlags { get; set; }

    [JsonProperty("terms_agreed_at")]
    public DateTime? TermsAgreedAt { get; set; }

    [JsonProperty("privacy_agreed_at")]
    public DateTime? PrivacyAgreedAt { get; set; }

    [JsonProperty("last_active_at")]
    public DateTime? LastActiveAt { get; set; }

    [JsonProperty("last_active_ip")]
    public string? LastActiveIp { get; set; }

    [JsonProperty("temp_banned_until")]
    public DateTime? TempBannedUntil { get; set; }

    [JsonProperty("pending_deletion_at")]
    public DateTime? PendingDeletionAt { get; set; }

    [JsonProperty("deletion_reason_code")]
    public int? DeletionReasonCode { get; set; }

    [JsonProperty("deletion_public_reason")]
    public string? DeletionPublicReason { get; set; }

    [JsonProperty("deletion_audit_log_reason")]
    public string? DeletionAuditLogReason { get; set; }

    [JsonProperty("acls")]
    public HashSet<string>? Acls { get; set; }

    [JsonProperty("first_refund_at")]
    public DateTime? FirstRefundAt { get; set; }

    [JsonProperty("beta_code_allowance")]
    public int BetaCodeAllowance { get; set; }

    [JsonProperty("beta_code_last_reset_at")]
    public DateTime? BetaCodeLastResetAt { get; set; }

    [JsonProperty("gift_inventory_server_seq")]
    public int? GiftInventoryServerSeq { get; set; }

    [JsonProperty("gift_inventory_client_seq")]
    public int? GiftInventoryClientSeq { get; set; }
}
