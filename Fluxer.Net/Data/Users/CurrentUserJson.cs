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
    public DateTimeOffset? PremiumSince { get; set; }

    /// <inheritdoc />
    [JsonProperty("premium_until")]
    public DateTimeOffset? PremiumUntil { get; set; }

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
    public DateTimeOffset? PasswordLastChangedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("has_ever_purchased")]
    public bool HasEverPurchased { get; set; }

    /// <inheritdoc />
    [JsonProperty("email_bounced")]
    public bool EmailBounced { get; set; }

    /// <inheritdoc />
    [JsonProperty("authenticator_types")]
    public HashSet<int>? AuthenticatorTypes { get; set; }

    /// <inheritdoc />
    public string? GetBannerUrl(int size = 600)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"https://fluxerusercontent.com/banners/{Id}/{BannerHash}.png?size={size}";
    }
}
