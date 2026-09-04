namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentUser : User, ICurrentUser, IUserProfile
{
    /// <inheritdoc />
    public bool IsStaff { get; private set; }

    /// <inheritdoc />
    public HashSet<string>? Acls { get; private set; }

    /// <inheritdoc />
    public HashSet<string>? Traits { get; private set; }

    /// <inheritdoc />
    public string? Email { get; private set; }

    /// <inheritdoc />
    public string? Phone { get; private set; }

    /// <inheritdoc />
    public string? Bio { get; private set; }

    /// <inheritdoc />
    public string? Pronouns { get; private set; }

    /// <inheritdoc />
    public int? AccentColor { get; private set; }

    /// <inheritdoc />
    public string? BannerHash { get; private set; }

    /// <inheritdoc />
    public int? BannerColor { get; private set; }

    /// <inheritdoc />
    public bool IsMfaEnabled { get; private set; }

    /// <inheritdoc />
    public bool IsEmailVerified { get; private set; }

    /// <inheritdoc />
    public PremiumType PremiumType { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? PremiumSince { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? PremiumUntil { get; private set; }

    /// <inheritdoc />
    public bool PremiumWillCancel { get; private set; }

    /// <inheritdoc />
    public string? PremiumBillingCycle { get; private set; }

    /// <inheritdoc />
    public int? PremiumLifetimeSequence { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? PasswordLastChangedAt { get; private set; }

    /// <inheritdoc />
    public bool HasEverPurchased { get; private set; }

    /// <inheritdoc />
    public bool EmailBounced { get; private set; }

    /// <inheritdoc />
    public HashSet<int>? AuthenticatorTypes { get; private set; }

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

    /// <summary>
    /// Create a CurrentUser object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
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
