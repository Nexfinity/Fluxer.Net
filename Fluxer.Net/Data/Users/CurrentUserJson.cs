using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentUserJson : UserJson, IUserProfile
{
    /// <inheritdoc />
    [JsonProperty("is_staff")]
    public bool IsStaff { get; set; }

    /// <inheritdoc />
    [JsonProperty("acls")]
    public HashSet<string>? Acls { get; set; }

    /// <inheritdoc />
    [JsonProperty("traits")]
    public HashSet<string>? Traits { get; set; }

    /// <inheritdoc />
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <inheritdoc />
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    /// <inheritdoc />
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <inheritdoc />
    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    /// <inheritdoc />
    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner_color")]
    public int? BannerColor { get; set; }

    /// <inheritdoc />
    [JsonProperty("mfa_enabled")]
    public bool IsMfaEnabled { get; set; }

    /// <inheritdoc />
    [JsonProperty("email_verified")]
    public bool IsEmailVerified { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_type")]
    public PremiumType PremiumType { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_since")]
    public DateTime? PremiumSince { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_until")]
    public DateTime? PremiumUntil { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_will_cancel")]
    public bool PremiumWillCancel { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_billing_cycle")]
    public string? PremiumBillingCycle { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_lifetime_sequence")]
    public int? PremiumLifetimeSequence { get; set; }

    /// <inheritdoc />
    [JsonProperty("password_last_changed_at")]
    public DateTime? PasswordLastChangedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("has_ever_purchased")]
    public bool HasEverPurchased { get; set; }

    /// <inheritdoc />
    [JsonProperty("email_bounced")]
    public bool EmailBounced { get; set; }

    /// <inheritdoc />
    [JsonProperty("authenticator_types")]
    public HashSet<int>? AuthenticatorTypes { get; set; }



    ///// <inheritdoc />
    //[JsonProperty("password_hash")]
    //public string? PasswordHash { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("totp_secret")]
    //public string? TotpSecret { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("date_of_birth")]
    //public string? DateOfBirth { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("locale")]
    //public string? Locale { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("stripe_subscription_id")]
    //public string? StripeSubscriptionId { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("stripe_customer_id")]
    //public string? StripeCustomerId { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("suspicious_activity_flags")]
    //public int SuspiciousActivityFlags { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("terms_agreed_at")]
    //public DateTime? TermsAgreedAt { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("privacy_agreed_at")]
    //public DateTime? PrivacyAgreedAt { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("last_active_at")]
    //public DateTime? LastActiveAt { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("last_active_ip")]
    //public string? LastActiveIp { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("temp_banned_until")]
    //public DateTime? TempBannedUntil { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("pending_deletion_at")]
    //public DateTime? PendingDeletionAt { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("deletion_reason_code")]
    //public int? DeletionReasonCode { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("deletion_public_reason")]
    //public string? DeletionPublicReason { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("deletion_audit_log_reason")]
    //public string? DeletionAuditLogReason { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("first_refund_at")]
    //public DateTime? FirstRefundAt { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("beta_code_allowance")]
    //public int BetaCodeAllowance { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("beta_code_last_reset_at")]
    //public DateTime? BetaCodeLastResetAt { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("gift_inventory_server_seq")]
    //public int? GiftInventoryServerSeq { get; set; }

    ///// <inheritdoc />
    //[JsonProperty("gift_inventory_client_seq")]
    //public int? GiftInventoryClientSeq { get; set; }
}
