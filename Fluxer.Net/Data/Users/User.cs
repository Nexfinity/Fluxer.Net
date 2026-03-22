namespace Fluxer.Net;

/// <inheritdoc />
public class User : Entity, IUser
{
    /// <inheritdoc />
    public ulong Id { get; set; }

    /// <inheritdoc />
    public string Username { get; set; }

    /// <inheritdoc />
    public int Discriminator { get; set; }

    /// <inheritdoc />
    public bool IsBot { get; set; }

    /// <inheritdoc />
    public bool IsSystem { get; set; }

    /// <inheritdoc />
    public string? Email { get; set; }

    /// <inheritdoc />
    public bool EmailVerified { get; set; }

    /// <inheritdoc />
    public bool EmailBounced { get; set; }

    /// <inheritdoc />
    public string? Phone { get; set; }

    /// <inheritdoc />
    public string? PasswordHash { get; set; }

    /// <inheritdoc />
    public DateTime? PasswordLastChangedAt { get; set; }

    /// <inheritdoc />
    public string? TotpSecret { get; set; }

    /// <inheritdoc />
    public HashSet<int>? AuthenticatorTypes { get; set; }

    /// <inheritdoc />
    public string? AvatarHash { get; set; }

    /// <inheritdoc />
    public string? BannerHash { get; set; }

    /// <inheritdoc />
    public string? Bio { get; set; }

    /// <inheritdoc />
    public string? Pronouns { get; set; }

    /// <inheritdoc />
    public int? AccentColor { get; set; }

    /// <inheritdoc />
    public string? DateOfBirth { get; set; }

    /// <inheritdoc />
    public string? Locale { get; set; }

    /// <inheritdoc />
    public UserFlags Flags { get; set; }

    /// <inheritdoc />
    public int? PremiumType { get; set; }

    /// <inheritdoc />
    public DateTime? PremiumSince { get; set; }

    /// <inheritdoc />
    public DateTime? PremiumUntil { get; set; }

    /// <inheritdoc />
    public bool PremiumWillCancel { get; set; }

    /// <inheritdoc />
    public string? PremiumBillingCycle { get; set; }

    /// <inheritdoc />
    public int? PremiumLifetimeSequence { get; set; }

    /// <inheritdoc />
    public string? StripeSubscriptionId { get; set; }

    /// <inheritdoc />
    public string? StripeCustomerId { get; set; }

    /// <inheritdoc />
    public bool HasEverPurchased { get; set; }

    /// <inheritdoc />
    public int SuspiciousActivityFlags { get; set; }

    /// <inheritdoc />
    public DateTime? TermsAgreedAt { get; set; }

    /// <inheritdoc />
    public DateTime? PrivacyAgreedAt { get; set; }

    /// <inheritdoc />
    public DateTime? LastActiveAt { get; set; }

    /// <inheritdoc />
    public string? LastActiveIp { get; set; }

    /// <inheritdoc />
    public DateTime? TempBannedUntil { get; set; }

    /// <inheritdoc />
    public DateTime? PendingDeletionAt { get; set; }

    /// <inheritdoc />
    public int? DeletionReasonCode { get; set; }

    /// <inheritdoc />
    public string? DeletionPublicReason { get; set; }

    /// <inheritdoc />
    public string? DeletionAuditLogReason { get; set; }

    /// <inheritdoc />
    public HashSet<string>? Acls { get; set; }

    /// <inheritdoc />
    public DateTime? FirstRefundAt { get; set; }

    /// <inheritdoc />
    public int BetaCodeAllowance { get; set; }

    /// <inheritdoc />
    public DateTime? BetaCodeLastResetAt { get; set; }

    /// <inheritdoc />
    public int? GiftInventoryServerSeq { get; set; }

    /// <inheritdoc />
    public int? GiftInventoryClientSeq { get; set; }

    internal User(BaseClient client) : base(client)
    {

    }

    public static User Create(BaseClient client, UserJson json)
    {
        var data = new User(client);
        data.Update(json);
        return data;
    }

    internal void Update(UserJson json)
    {
        Id = json.Id;
        Username = json.Username;
        Discriminator = json.Discriminator;
        IsBot = json.IsBot;
        IsSystem = json.IsSystem;
        Email = json.Email;
        EmailVerified = json.EmailVerified;
        EmailBounced = json.EmailBounced;
        Phone = json.Phone;
        PasswordHash = json.PasswordHash;
        PasswordLastChangedAt = json.PasswordLastChangedAt;
        TotpSecret = json.TotpSecret;
        AuthenticatorTypes = json.AuthenticatorTypes;
        AvatarHash = json.AvatarHash;
        BannerHash = json.BannerHash;
        Bio = json.Bio;
        Pronouns = json.Pronouns;
        AccentColor = json.AccentColor;
        DateOfBirth = json.DateOfBirth;
        Locale = json.Locale;
        Flags = json.Flags;
        PremiumType = json.PremiumType;
        PremiumSince = json.PremiumSince;
        PremiumUntil = json.PremiumUntil;
        PremiumWillCancel = json.PremiumWillCancel;
        PremiumBillingCycle = json.PremiumBillingCycle;
        PremiumLifetimeSequence = json.PremiumLifetimeSequence;
        StripeSubscriptionId = json.StripeSubscriptionId;
        StripeCustomerId = json.StripeCustomerId;
        HasEverPurchased = json.HasEverPurchased;
        SuspiciousActivityFlags = json.SuspiciousActivityFlags;
        TermsAgreedAt = json.TermsAgreedAt;
        PrivacyAgreedAt = json.PrivacyAgreedAt;
        LastActiveAt = json.LastActiveAt;
        LastActiveIp = json.LastActiveIp;
        TempBannedUntil = json.TempBannedUntil;
        PendingDeletionAt = json.PendingDeletionAt;
        DeletionReasonCode = json.DeletionReasonCode;
        DeletionPublicReason = json.DeletionPublicReason;
        DeletionAuditLogReason = json.DeletionAuditLogReason;
        Acls = json.Acls;
        FirstRefundAt = json.FirstRefundAt;
        BetaCodeAllowance = json.BetaCodeAllowance;
        BetaCodeLastResetAt = json.BetaCodeLastResetAt;
        GiftInventoryServerSeq = json.GiftInventoryServerSeq;
        GiftInventoryClientSeq = json.GiftInventoryClientSeq;
    }
}
