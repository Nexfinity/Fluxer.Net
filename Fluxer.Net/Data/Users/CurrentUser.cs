namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentUser : User, IUserProfile
{
    /// <inheritdoc />
    public bool IsStaff { get; internal set; }

    /// <inheritdoc />
    public HashSet<string>? Acls { get; internal set; }

    /// <inheritdoc />
    public HashSet<string>? Traits { get; internal set; }

    /// <inheritdoc />
    public string? Email { get; internal set; }

    /// <inheritdoc />
    public string? Phone { get; internal set; }

    /// <inheritdoc />
    public string? Bio { get; internal set; }

    /// <inheritdoc />
    public string? Pronouns { get; internal set; }

    /// <inheritdoc />
    public int? AccentColor { get; internal set; }

    /// <inheritdoc />
    public string? BannerHash { get; internal set; }

    /// <inheritdoc />
    public int? BannerColor { get; internal set; }

    /// <inheritdoc />
    public bool IsMfaEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsEmailVerified { get; internal set; }

    /// <inheritdoc />
    public PremiumType PremiumType { get; internal set; }

    /// <inheritdoc />
    public DateTime? PremiumSince { get; internal set; }

    /// <inheritdoc />
    public DateTime? PremiumUntil { get; internal set; }

    /// <inheritdoc />
    public bool PremiumWillCancel { get; internal set; }

    /// <inheritdoc />
    public string? PremiumBillingCycle { get; internal set; }

    /// <inheritdoc />
    public int? PremiumLifetimeSequence { get; internal set; }

    /// <inheritdoc />
    public DateTime? PasswordLastChangedAt { get; internal set; }

    /// <inheritdoc />
    public bool HasEverPurchased { get; internal set; }

    /// <inheritdoc />
    public bool EmailBounced { get; internal set; }

    /// <inheritdoc />
    public HashSet<int>? AuthenticatorTypes { get; internal set; }



    ///// <inheritdoc />
    //public string? PasswordHash { get; internal set; }

    ///// <inheritdoc />
    //public string? TotpSecret { get; internal set; }

    ///// <inheritdoc />
    //public string? DateOfBirth { get; internal set; }

    ///// <inheritdoc />
    //public string? Locale { get; internal set; }

    ///// <inheritdoc />
    //public string? StripeSubscriptionId { get; internal set; }

    ///// <inheritdoc />
    //public string? StripeCustomerId { get; internal set; }

    ///// <inheritdoc />
    //public int SuspiciousActivityFlags { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? TermsAgreedAt { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? PrivacyAgreedAt { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? LastActiveAt { get; internal set; }

    ///// <inheritdoc />
    //public string? LastActiveIp { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? TempBannedUntil { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? PendingDeletionAt { get; internal set; }

    ///// <inheritdoc />
    //public int? DeletionReasonCode { get; internal set; }

    ///// <inheritdoc />
    //public string? DeletionPublicReason { get; internal set; }

    ///// <inheritdoc />
    //public string? DeletionAuditLogReason { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? FirstRefundAt { get; internal set; }

    ///// <inheritdoc />
    //public int BetaCodeAllowance { get; internal set; }

    ///// <inheritdoc />
    //public DateTime? BetaCodeLastResetAt { get; internal set; }

    ///// <inheritdoc />
    //public int? GiftInventoryServerSeq { get; internal set; }

    ///// <inheritdoc />
    //public int? GiftInventoryClientSeq { get; internal set; }

    internal CurrentUser(FluxerBaseClient client) : base(client)
    {

    }

    public static CurrentUser Create(FluxerBaseClient client, CurrentUserJson json)
    {
        var data = new CurrentUser(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, CurrentUserJson json)
    {
        base.Update(json);
        IsStaff = json.IsStaff;
        Acls = json.Acls;
        Traits = json.Traits;
        Email = json.Email;
        Phone = json.Phone;
        Bio = json.Bio;
        Pronouns = json.Pronouns;
        AccentColor = json.AccentColor;
        BannerHash = json.BannerHash;
        BannerColor = json.BannerColor;
        IsMfaEnabled = json.IsMfaEnabled;
        IsEmailVerified = json.IsEmailVerified;
        PremiumType = json.PremiumType;
        PremiumSince = json.PremiumSince;
        PremiumUntil = json.PremiumUntil;
        PremiumWillCancel = json.PremiumWillCancel;
        PremiumBillingCycle = json.PremiumBillingCycle;
        PremiumLifetimeSequence = json.PremiumLifetimeSequence;
        PasswordLastChangedAt = json.PasswordLastChangedAt;
        HasEverPurchased = json.HasEverPurchased;
        EmailBounced = json.EmailBounced;
        AuthenticatorTypes = json.AuthenticatorTypes;

        //PasswordHash = json.PasswordHash;
        //TotpSecret = json.TotpSecret;
        //DateOfBirth = json.DateOfBirth;
        //Locale = json.Locale;
        //StripeSubscriptionId = json.StripeSubscriptionId;
        //StripeCustomerId = json.StripeCustomerId;
        //SuspiciousActivityFlags = json.SuspiciousActivityFlags;
        //TermsAgreedAt = json.TermsAgreedAt;
        //PrivacyAgreedAt = json.PrivacyAgreedAt;
        //LastActiveAt = json.LastActiveAt;
        //LastActiveIp = json.LastActiveIp;
        //TempBannedUntil = json.TempBannedUntil;
        //PendingDeletionAt = json.PendingDeletionAt;
        //DeletionReasonCode = json.DeletionReasonCode;
        //DeletionPublicReason = json.DeletionPublicReason;
        //DeletionAuditLogReason = json.DeletionAuditLogReason;
        //FirstRefundAt = json.FirstRefundAt;
        //BetaCodeAllowance = json.BetaCodeAllowance;
        //BetaCodeLastResetAt = json.BetaCodeLastResetAt;
        //GiftInventoryServerSeq = json.GiftInventoryServerSeq;
        //GiftInventoryClientSeq = json.GiftInventoryClientSeq;
    }
}
