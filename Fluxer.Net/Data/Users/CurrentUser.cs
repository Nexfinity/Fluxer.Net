namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentUser : User, ICurrentUser, IUserProfile
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

    /// <inheritdoc />
    public string? GetBannerUrl(int size = 600)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"{Client.Config.MediaUrl}/banners/{Id}/{BannerHash}.png?size={size}";
    }

    internal CurrentUser(FluxerBaseClient client) : base(client)
    {

    }

    public static CurrentUser Create(FluxerBaseClient client, CurrentUserJson json)
    {
        CurrentUser data = new CurrentUser(client);
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
    }
}
