using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class InstanceJson : IInstance
{
    /// <inheritdoc />
    [JsonProperty("api_code_version")]
    public int ApiVersion { get; set; }

    /// <inheritdoc />
    [JsonProperty("endpoints")]
    public InstanceEndpointsJson Endpoints { get; set; }

    /// <inheritdoc />
    [JsonProperty("captcha")]
    public InstanceCaptchaJson Captcha { get; set; }

    /// <inheritdoc />
    [JsonProperty("features")]
    public InstanceFeaturesJson Features { get; set; }

    /// <inheritdoc />
    [JsonProperty("gif")]
    public InstanceGifsJson Gifs { get; set; }

    /// <inheritdoc />
    [JsonProperty("sso")]
    public InstanceSSOJson SSO { get; set; }

    /// <inheritdoc />
    [JsonProperty("registration")]
    public InstanceRegistrationJson Registration { get; set; }

    /// <inheritdoc />
    [JsonProperty("community")]
    public InstanceCommunityJson Community { get; set; }

    /// <inheritdoc />
    [JsonProperty("services")]
    public InstanceServicesJson Services { get; set; }

    /// <inheritdoc />
    [JsonProperty("limits")]
    public InstanceLimitsJson Limits { get; set; }

    /// <inheritdoc />
    [JsonProperty("push")]
    public InstancePushJson Push { get; set; }

    /// <inheritdoc />
    [JsonProperty("app_public")]
    public InstanceAppJson App { get; set; }

    IInstanceEndpoints IInstance.Endpoints => Endpoints;

    IInstanceCaptcha IInstance.Captcha => Captcha;

    IInstanceFeatures IInstance.Features => Features;

    IInstanceGifs IInstance.Gifs => Gifs;

    IInstanceSSO IInstance.SSO => SSO;

    IInstanceRegistration IInstance.Registration => Registration;

    IInstanceCommunity IInstance.Community => Community;

    IInstanceServices IInstance.Services => Services;

    IInstanceLimits IInstance.Limits => Limits;

    IInstancePush IInstance.Push => Push;

    IInstanceApp IInstance.App => App;
}

/// <inheritdoc />
public class InstanceEndpointsJson : IInstanceEndpoints
{
    /// <inheritdoc />
    [JsonProperty("api")]
    public string Api { get; set; }

    /// <inheritdoc />
    [JsonProperty("api_client")]
    public string ApiClient { get; set; }

    /// <inheritdoc />
    [JsonProperty("api_public")]
    public string ApiPublic { get; set; }

    /// <inheritdoc />
    [JsonProperty("gateway")]
    public string Gateway { get; set; }

    /// <inheritdoc />
    [JsonProperty("media")]
    public string Media { get; set; }

    /// <inheritdoc />
    [JsonProperty("static_cdn")]
    public string Static { get; set; }

    /// <inheritdoc />
    [JsonProperty("marketing")]
    public string Marketing { get; set; }

    /// <inheritdoc />
    [JsonProperty("admin")]
    public string Admin { get; set; }

    /// <inheritdoc />
    [JsonProperty("invite")]
    public string Invite { get; set; }

    /// <inheritdoc />
    [JsonProperty("gift")]
    public string Gift { get; set; }

    /// <inheritdoc />
    [JsonProperty("webapp")]
    public string WebApp { get; set; }
}

/// <inheritdoc />
public class InstanceCaptchaJson : IInstanceCaptcha
{

    /// <inheritdoc />
    [JsonProperty("provider")]
    public string? Provider { get; set; }

    /// <inheritdoc />
    [JsonProperty("hcaptcha_site_key")]
    public string? HCaptchaSiteKey { get; set; }

    /// <inheritdoc />
    [JsonProperty("turnstile_site_key")]
    public string? TurnstileSiteKey { get; set; }
}

/// <inheritdoc />
public class InstanceFeaturesJson : IInstanceFeatures
{
    /// <inheritdoc />
    [JsonProperty("voice_enabled")]
    public bool IsVoiceEnabled { get; set; }

    /// <inheritdoc />
    [JsonProperty("stripe_enabled")]
    public bool IsStripeEnabled { get; set; }

    /// <inheritdoc />
    [JsonProperty("self_hosted")]
    public bool IsSelfHosted { get; set; }

    /// <inheritdoc />
    [JsonProperty("presigned_attachment_uploads")]
    public bool IsPresignedAttachmentUploads { get; set; }

    /// <inheritdoc />
    [JsonProperty("emails_enabled")]
    public bool IsEmailsEnabled { get; set; }
}

/// <inheritdoc />
public class InstanceGifsJson : IInstanceGifs
{
    /// <inheritdoc />
    [JsonProperty("provider")]
    public string? Provider { get; set; }

    /// <inheritdoc />
    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    /// <inheritdoc />
    [JsonProperty("attribution_required")]
    public bool IsAttributionRequired { get; set; }
}


/// <inheritdoc />
public class InstanceSSOJson : IInstanceSSO
{
    /// <inheritdoc />
    [JsonProperty("enabled")]
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    [JsonProperty("enforced")]
    public bool IsEnforced { get; set; }

    /// <inheritdoc />
    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    /// <inheritdoc />
    [JsonProperty("redirect_url")]
    public string RedirectUrl { get; set; }
}

/// <inheritdoc />
public class InstanceRegistrationJson : IInstanceRegistration
{
    /// <inheritdoc />
    [JsonProperty("mode")]
    public string Mode { get; set; }

    /// <inheritdoc />
    [JsonProperty("admin_registration_urls_enabled")]
    public bool IsAdminRegistrationEnabled { get; set; }
}

/// <inheritdoc />
public class InstanceCommunityJson : IInstanceCommunity
{
    /// <inheritdoc />
    [JsonProperty("single_community")]
    public bool IsSingleCommunity { get; set; }

    /// <inheritdoc />
    [JsonProperty("single_community_guild_id")]
    public ulong? SingleCommunityGuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("direct_messages_disabled")]
    public bool IsDirectMessagesDisabled { get; set; }
}

/// <inheritdoc />
public class InstanceServicesJson : IInstanceServices
{
    /// <inheritdoc />
    [JsonProperty("gif_enabled")]
    public bool IsGifEnabled { get; set; }

    /// <inheritdoc />
    [JsonProperty("youtube_enabled")]
    public bool IsYouTubeEnabled { get; set; }

    /// <inheritdoc />
    [JsonProperty("bluesky_enabled")]
    public bool IsBlueSkyEnabled { get; set; }
}

/// <inheritdoc />
public class InstanceLimitsJson : IInstanceLimits
{
    /// <inheritdoc />
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <inheritdoc />
    [JsonProperty("traitsDefinition")]
    public string[] Traits { get; set; }

    /// <inheritdoc />
    [JsonProperty("rules")]
    public InstanceRuleJson[] Rules { get; set; }

    IInstanceRule[] IInstanceLimits.Rules => Rules;
}

public class InstanceRuleJson : IInstanceRule
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("overrides")]
    public Dictionary<string, int> Overrides { get; set; }

    IDictionary<string, int> IInstanceRule.Overrides => Overrides;
}

/// <inheritdoc />
public class InstancePushJson : IInstancePush
{
    /// <inheritdoc />
    [JsonProperty("public_vapid_key")]
    public string PublicVapidKey { get; set; }
}

/// <inheritdoc />
public class InstanceAppJson : IInstanceApp
{
    /// <inheritdoc />
    [JsonProperty("branding")]
    public InstanceAppBrandingJson Branding { get; set; }

    /// <inheritdoc />
    [JsonProperty("setup")]
    public InstanceAppSetupJson Setup { get; set; }

    /// <inheritdoc />
    [JsonProperty("legal")]
    public InstanceAppLegalJson Legal { get; set; }

    /// <inheritdoc />
    [JsonProperty("registration")]
    public InstanceAppRegistrationJson Registration { get; set; }

    IInstanceAppBranding IInstanceApp.Branding => Branding;

    IInstanceAppSetup IInstanceApp.Setup => Setup;

    IInstanceAppLegal IInstanceApp.Legal => Legal;

    IInstanceAppRegistration IInstanceApp.Registration => Registration;
}

/// <inheritdoc />
public class InstanceAppBrandingJson : IInstanceAppBranding
{
    /// <inheritdoc />
    [JsonProperty("product_name")]
    public string ProductName { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("symbol_url")]
    public string? SymbolUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("logo_url")]
    public string? LogoUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("wordmark_url")]
    public string? WordmarkUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("favicon_url")]
    public string? FaviconUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("theme_color")]
    public string ThemeColor { get; set; }
}


/// <inheritdoc />
public class InstanceAppSetupJson : IInstanceAppSetup
{
    /// <inheritdoc />
    [JsonProperty("configured")]
    public bool IsConfigured { get; set; }

    /// <inheritdoc />
    [JsonProperty("admin_url")]
    public string AdminUrl { get; set; }
}

/// <inheritdoc />
public class InstanceAppLegalJson : IInstanceAppLegal
{
    /// <inheritdoc />
    [JsonProperty("terms_url")]
    public string? TermsUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("privacy_url")]
    public string? PrivacyUrl { get; set; }
}

/// <inheritdoc />
public class InstanceAppRegistrationJson : IInstanceAppRegistration
{
    /// <inheritdoc />
    [JsonProperty("collect_date_of_birth")]
    public bool CollectDateOfBirth { get; set; }
}