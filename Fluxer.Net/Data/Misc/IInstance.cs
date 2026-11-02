namespace Fluxer.Net;

/// <summary>
/// The configuration of a Fluxer instance.
/// </summary>
public interface IInstance
{
    /// <summary>
    /// The version of the API code the instance runs.
    /// </summary>
    int ApiVersion { get; }

    /// <summary>
    /// The base URLs of the instance's services.
    /// </summary>
    IInstanceEndpoints Endpoints { get; }

    /// <summary>
    /// The captcha settings of the instance.
    /// </summary>
    IInstanceCaptcha Captcha { get; }

    /// <summary>
    /// The features the instance has enabled.
    /// </summary>
    IInstanceFeatures Features { get; }

    /// <summary>
    /// The GIF provider settings of the instance.
    /// </summary>
    IInstanceGifs Gifs { get; }

    /// <summary>
    /// The single sign-on settings of the instance.
    /// </summary>
    IInstanceSSO SSO { get; }

    /// <summary>
    /// The account registration settings of the instance.
    /// </summary>
    IInstanceRegistration Registration { get; }

    /// <summary>
    /// The community settings of the instance.
    /// </summary>
    IInstanceCommunity Community { get; }

    /// <summary>
    /// The third-party services the instance has enabled.
    /// </summary>
    IInstanceServices Services { get; }

    /// <summary>
    /// The limits that apply to users of the instance.
    /// </summary>
    IInstanceLimits Limits { get; }

    /// <summary>
    /// The push notification settings of the instance.
    /// </summary>
    IInstancePush Push { get; }

    /// <summary>
    /// The settings of the instance's web app.
    /// </summary>
    IInstanceApp App { get; }
}
/// <summary>
/// The base URLs of an instance's services.
/// </summary>
public interface IInstanceEndpoints
{
    /// <summary>
    /// The base URL of the API.
    /// </summary>
    string Api { get; }

    /// <summary>
    /// The base URL of the API used by clients.
    /// </summary>
    string ApiClient { get; }

    /// <summary>
    /// The base URL of the public API.
    /// </summary>
    string ApiPublic { get; }

    /// <summary>
    /// The URL of the gateway.
    /// </summary>
    string Gateway { get; }

    /// <summary>
    /// The base URL of media.
    /// </summary>
    string Media { get; }

    /// <summary>
    /// The base URL of static assets.
    /// </summary>
    string Static { get; }

    /// <summary>
    /// The base URL of the marketing site.
    /// </summary>
    string Marketing { get; }

    /// <summary>
    /// The base URL of the admin site.
    /// </summary>
    string Admin { get; }

    /// <summary>
    /// The base URL of invite links.
    /// </summary>
    string Invite { get; }

    /// <summary>
    /// The base URL of gift links.
    /// </summary>
    string Gift { get; }

    /// <summary>
    /// The base URL of the web app.
    /// </summary>
    string WebApp { get; }
}
/// <summary>
/// The captcha settings of an instance.
/// </summary>
public interface IInstanceCaptcha
{
    /// <summary>
    /// The captcha provider the instance uses, or null when it has none.
    /// </summary>
    string? Provider { get; }

    /// <summary>
    /// The site key for hCaptcha, or null when it is not set.
    /// </summary>
    string? HCaptchaSiteKey { get; }

    /// <summary>
    /// The site key for Cloudflare Turnstile, or null when it is not set.
    /// </summary>
    string? TurnstileSiteKey { get; }
}
/// <summary>
/// The features an instance has enabled.
/// </summary>
public interface IInstanceFeatures
{
    /// <summary>
    /// Whether voice is enabled.
    /// </summary>
    bool IsVoiceEnabled { get; }

    /// <summary>
    /// Whether Stripe billing is enabled.
    /// </summary>
    bool IsStripeEnabled { get; }

    /// <summary>
    /// Whether the instance is self-hosted.
    /// </summary>
    bool IsSelfHosted { get; }

    /// <summary>
    /// Whether attachments are uploaded with presigned URLs.
    /// </summary>
    bool IsPresignedAttachmentUploads { get; }

    /// <summary>
    /// Whether the instance sends emails.
    /// </summary>
    bool IsEmailsEnabled { get; }
}
/// <summary>
/// The GIF provider settings of an instance.
/// </summary>
public interface IInstanceGifs
{
    /// <summary>
    /// The GIF provider, or null when none is set.
    /// </summary>
    string? Provider { get; }

    /// <summary>
    /// The name of the GIF provider as shown to users, or null when none is set.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// Whether GIFs must show the provider's attribution.
    /// </summary>
    bool IsAttributionRequired { get; }
}
/// <summary>
/// The single sign-on settings of an instance.
/// </summary>
public interface IInstanceSSO
{
    /// <summary>
    /// Whether single sign-on is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Whether single sign-on is required to sign in.
    /// </summary>
    bool IsEnforced { get; }

    /// <summary>
    /// The name of the single sign-on provider as shown to users, or null when none is set.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// The URL the single sign-on flow redirects to.
    /// </summary>
    string RedirectUrl { get; }
}
/// <summary>
/// The account registration settings of an instance.
/// </summary>
public interface IInstanceRegistration
{
    /// <summary>
    /// The registration mode of the instance.
    /// </summary>
    string Mode { get; }

    /// <summary>
    /// Whether registration URLs created by admins are enabled.
    /// </summary>
    bool IsAdminRegistrationEnabled { get; }
}
/// <summary>
/// The community settings of an instance.
/// </summary>
public interface IInstanceCommunity
{
    /// <summary>
    /// Whether the instance is a single community.
    /// </summary>
    bool IsSingleCommunity { get; }

    /// <summary>
    /// The guild of the single community, or null when the instance is not one.
    /// </summary>
    ulong? SingleCommunityGuildId { get; }

    /// <summary>
    /// Whether direct messages are disabled.
    /// </summary>
    bool IsDirectMessagesDisabled { get; }
}
/// <summary>
/// The third-party services an instance has enabled.
/// </summary>
public interface IInstanceServices
{
    /// <summary>
    /// Whether GIF search is enabled.
    /// </summary>
    bool IsGifEnabled { get; }

    /// <summary>
    /// Whether the YouTube service is enabled.
    /// </summary>
    bool IsYouTubeEnabled { get; }

    /// <summary>
    /// Whether the Bluesky service is enabled.
    /// </summary>
    bool IsBlueSkyEnabled { get; }
}
/// <summary>
/// The limits that apply to users of an instance.
/// </summary>
public interface IInstanceLimits
{
    /// <summary>
    /// The version of the limits.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// The trait definitions the limit rules use.
    /// </summary>
    string[] Traits { get; }

    /// <summary>
    /// The limit rules.
    /// </summary>
    IInstanceRule[] Rules { get; }
}
/// <summary>
/// A limit rule and the limit values it overrides.
/// </summary>
public interface IInstanceRule
{
    /// <summary>
    /// The id of the rule.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The limit values the rule sets, by limit name.
    /// </summary>
    IDictionary<string, long> Overrides { get; }
}
/// <summary>
/// The push notification settings of an instance.
/// </summary>
public interface IInstancePush
{
    /// <summary>
    /// The public VAPID key for web push subscriptions.
    /// </summary>
    string PublicVapidKey { get; }
}
/// <summary>
/// The settings of an instance's web app.
/// </summary>
public interface IInstanceApp
{
    /// <summary>
    /// The branding of the web app.
    /// </summary>
    IInstanceAppBranding Branding { get; }

    /// <summary>
    /// The setup state of the instance.
    /// </summary>
    IInstanceAppSetup Setup { get; }

    /// <summary>
    /// The legal links of the web app.
    /// </summary>
    IInstanceAppLegal Legal { get; }

    /// <summary>
    /// The registration settings of the web app.
    /// </summary>
    IInstanceAppRegistration Registration { get; }
}
/// <summary>
/// The branding of an instance's web app.
/// </summary>
public interface IInstanceAppBranding
{
    /// <summary>
    /// The product name shown in the web app.
    /// </summary>
    string ProductName { get; }

    /// <summary>
    /// The URL of the icon, or null when none is set.
    /// </summary>
    string? IconUrl { get; }

    /// <summary>
    /// The URL of the symbol, or null when none is set.
    /// </summary>
    string? SymbolUrl { get; }

    /// <summary>
    /// The URL of the logo, or null when none is set.
    /// </summary>
    string? LogoUrl { get; }

    /// <summary>
    /// The URL of the wordmark, or null when none is set.
    /// </summary>
    string? WordmarkUrl { get; }

    /// <summary>
    /// The URL of the favicon, or null when none is set.
    /// </summary>
    string? FaviconUrl { get; }

    /// <summary>
    /// The theme color of the web app.
    /// </summary>
    string ThemeColor { get; }
}
/// <summary>
/// The setup state of an instance.
/// </summary>
public interface IInstanceAppSetup
{
    /// <summary>
    /// Whether the instance has been configured.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// The URL of the admin setup page.
    /// </summary>
    string AdminUrl { get; }
}
/// <summary>
/// The legal links of an instance's web app.
/// </summary>
public interface IInstanceAppLegal
{
    /// <summary>
    /// The URL of the terms of service, or null when none is set.
    /// </summary>
    string? TermsUrl { get; }

    /// <summary>
    /// The URL of the privacy policy, or null when none is set.
    /// </summary>
    string? PrivacyUrl { get; }
}
/// <summary>
/// The registration settings shown in an instance's web app.
/// </summary>
public interface IInstanceAppRegistration
{
    /// <summary>
    /// Whether registration asks for a date of birth.
    /// </summary>
    bool CollectDateOfBirth { get; }
}