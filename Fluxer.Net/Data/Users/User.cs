namespace Fluxer.Net;

/// <inheritdoc />
public class User : Entity, IUser
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Username { get; internal set; }

    /// <inheritdoc />
    public int Discriminator { get; internal set; }

    /// <inheritdoc />
    public bool IsBot { get; internal set; }

    /// <inheritdoc />
    public bool IsSystem { get; internal set; }

    /// <inheritdoc />
    public string? Email { get; internal set; }

    /// <inheritdoc />
    public bool EmailVerified { get; internal set; }

    /// <inheritdoc />
    public bool EmailBounced { get; internal set; }

    /// <inheritdoc />
    public string? Phone { get; internal set; }

    /// <inheritdoc />
    public string? PasswordHash { get; internal set; }

    /// <inheritdoc />
    public DateTime? PasswordLastChangedAt { get; internal set; }

    /// <inheritdoc />
    public string? TotpSecret { get; internal set; }

    /// <inheritdoc />
    public HashSet<int>? AuthenticatorTypes { get; internal set; }

    /// <inheritdoc />
    public string? AvatarHash { get; internal set; }

    /// <inheritdoc />
    public string? BannerHash { get; internal set; }

    /// <inheritdoc />
    public string? Bio { get; internal set; }

    /// <inheritdoc />
    public string? Pronouns { get; internal set; }

    /// <inheritdoc />
    public int? AccentColor { get; internal set; }

    /// <inheritdoc />
    public string? DateOfBirth { get; internal set; }

    /// <inheritdoc />
    public string? Locale { get; internal set; }

    /// <inheritdoc />
    public UserFlags Flags { get; internal set; }

    /// <inheritdoc />
    public int? PremiumType { get; internal set; }

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
    public string? StripeSubscriptionId { get; internal set; }

    /// <inheritdoc />
    public string? StripeCustomerId { get; internal set; }

    /// <inheritdoc />
    public bool HasEverPurchased { get; internal set; }

    /// <inheritdoc />
    public int SuspiciousActivityFlags { get; internal set; }

    /// <inheritdoc />
    public DateTime? TermsAgreedAt { get; internal set; }

    /// <inheritdoc />
    public DateTime? PrivacyAgreedAt { get; internal set; }

    /// <inheritdoc />
    public DateTime? LastActiveAt { get; internal set; }

    /// <inheritdoc />
    public string? LastActiveIp { get; internal set; }

    /// <inheritdoc />
    public DateTime? TempBannedUntil { get; internal set; }

    /// <inheritdoc />
    public DateTime? PendingDeletionAt { get; internal set; }

    /// <inheritdoc />
    public int? DeletionReasonCode { get; internal set; }

    /// <inheritdoc />
    public string? DeletionPublicReason { get; internal set; }

    /// <inheritdoc />
    public string? DeletionAuditLogReason { get; internal set; }

    /// <inheritdoc />
    public HashSet<string>? Acls { get; internal set; }

    /// <inheritdoc />
    public DateTime? FirstRefundAt { get; internal set; }

    /// <inheritdoc />
    public int BetaCodeAllowance { get; internal set; }

    /// <inheritdoc />
    public DateTime? BetaCodeLastResetAt { get; internal set; }

    /// <inheritdoc />
    public int? GiftInventoryServerSeq { get; internal set; }

    /// <inheritdoc />
    public int? GiftInventoryClientSeq { get; internal set; }

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
