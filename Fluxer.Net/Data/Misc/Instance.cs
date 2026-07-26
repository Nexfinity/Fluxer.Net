namespace Fluxer.Net;

/// <inheritdoc />
public class Instance : Entity, IInstance
{
    /// <inheritdoc />
    public int ApiVersion { get; internal set; }

    /// <inheritdoc />
    public InstanceEndpoints Endpoints { get; internal set; }

    /// <inheritdoc />
    public InstanceCaptcha Captcha { get; internal set; }

    /// <inheritdoc />
    public InstanceFeatures Features { get; internal set; }

    /// <inheritdoc />
    public InstanceGifs Gifs { get; internal set; }

    /// <inheritdoc />
    public InstanceSSO SSO { get; internal set; }

    /// <inheritdoc />
    public InstanceRegistration Registration { get; internal set; }

    /// <inheritdoc />
    public InstanceCommunity Community { get; internal set; }

    /// <inheritdoc />
    public InstanceServices Services { get; internal set; }

    /// <inheritdoc />
    public InstanceLimits Limits { get; internal set; }

    /// <inheritdoc />
    public InstancePush Push { get; internal set; }

    /// <inheritdoc />
    public InstanceApp App { get; internal set; }

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

    internal Instance(FluxerBaseClient client) : base(client)
    {

    }

    public static Instance Create(FluxerBaseClient client, InstanceJson json)
    {
        Instance data = new Instance(client)
        {
            ApiVersion = json.ApiVersion,
            Captcha = new InstanceCaptcha
            {
                HCaptchaSiteKey = json.Captcha.HCaptchaSiteKey,
                Provider = json.Captcha.Provider,
                TurnstileSiteKey = json.Captcha.TurnstileSiteKey
            },
            Community = new InstanceCommunity
            {
                IsDirectMessagesDisabled = json.Community.IsDirectMessagesDisabled,
                IsSingleCommunity = json.Community.IsSingleCommunity,
                SingleCommunityGuildId = json.Community.SingleCommunityGuildId,
            },
            Endpoints = new InstanceEndpoints
            {
                Admin = json.Endpoints.Admin,
                Api = json.Endpoints.Api,
                ApiClient = json.Endpoints.ApiClient,
                ApiPublic = json.Endpoints.ApiPublic,
                WebApp = json.Endpoints.WebApp,
                Cdn = json.Endpoints.Cdn,
                Gateway = json.Endpoints.Gateway,
                Gift = json.Endpoints.Gift,
                Invite = json.Endpoints.Invite,
                Marketing = json.Endpoints.Marketing,
                Media = json.Endpoints.Media,
            },
            Features = new InstanceFeatures
            {
                IsPresignedAttachmentUploads = json.Features.IsPresignedAttachmentUploads,
                IsEmailsEnabled = json.Features.IsEmailsEnabled,
                IsSelfHosted = json.Features.IsSelfHosted,
                IsStripeEnabled = json.Features.IsStripeEnabled,
                IsVoiceEnabled = json.Features.IsVoiceEnabled,
            },
            Gifs = new InstanceGifs
            {
                IsAttributionRequired = json.Gifs.IsAttributionRequired,
                DisplayName = json.Gifs.DisplayName,
                Provider = json.Gifs.Provider,
            },
            Limits = new InstanceLimits
            {
                Traits = json.Limits.Traits,
                Version = json.Limits.Version,
            },
            Push = new InstancePush
            {
                PublicVapidKey = json.Push.PublicVapidKey,
            },
            Registration = new InstanceRegistration
            {
                IsAdminRegistrationEnabled = json.Registration.IsAdminRegistrationEnabled,
                Mode = json.Registration.Mode,
            },
            Services = new InstanceServices
            {
                IsBlueSkyEnabled = json.Services.IsBlueSkyEnabled,
                IsGifEnabled = json.Services.IsGifEnabled,
                IsYouTubeEnabled = json.Services.IsYouTubeEnabled,
            },
            SSO = new InstanceSSO
            {
                DisplayName = json.SSO.DisplayName,
                IsEnabled = json.SSO.IsEnabled,
                IsEnforced = json.SSO.IsEnforced,
                RedirectUrl = json.SSO.RedirectUrl,
            },
            App = new InstanceApp
            {
                Setup = new InstanceAppSetup
                {
                    AdminUrl = json.App.Setup.AdminUrl,
                    IsConfigured = json.App.Setup.IsConfigured,
                },
                Branding = new InstanceAppBranding
                {
                    FaviconUrl = json.App.Branding.FaviconUrl,
                    IconUrl = json.App.Branding.IconUrl,
                    SymbolUrl = json.App.Branding.SymbolUrl,
                    LogoUrl = json.App.Branding.LogoUrl,
                    ProductName = json.App.Branding.ProductName,
                    ThemeColor = json.App.Branding.ThemeColor,
                    WordmarkUrl = json.App.Branding.WordmarkUrl,
                }
            }
        };
        return data;
    }
}

/// <inheritdoc />
public class InstanceEndpoints : IInstanceEndpoints
{
    /// <inheritdoc />
    public string Api { get; internal set; }

    /// <inheritdoc />
    public string ApiClient { get; internal set; }

    /// <inheritdoc />
    public string ApiPublic { get; internal set; }

    /// <inheritdoc />
    public string Gateway { get; internal set; }

    /// <inheritdoc />
    public string Media { get; internal set; }

    /// <inheritdoc />
    public string Cdn { get; internal set; }

    /// <inheritdoc />
    public string Marketing { get; internal set; }

    /// <inheritdoc />
    public string Admin { get; internal set; }

    /// <inheritdoc />
    public string Invite { get; internal set; }

    /// <inheritdoc />
    public string Gift { get; internal set; }

    /// <inheritdoc />
    public string WebApp { get; internal set; }
}

/// <inheritdoc />
public class InstanceCaptcha : IInstanceCaptcha
{
    /// <inheritdoc />
    public string? Provider { get; internal set; }

    /// <inheritdoc />
    public string? HCaptchaSiteKey { get; internal set; }

    /// <inheritdoc />
    public string? TurnstileSiteKey { get; internal set; }
}

/// <inheritdoc />
public class InstanceFeatures : IInstanceFeatures
{
    /// <inheritdoc />
    public bool IsVoiceEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsStripeEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsSelfHosted { get; internal set; }

    /// <inheritdoc />
    public bool IsPresignedAttachmentUploads { get; internal set; }

    /// <inheritdoc />
    public bool IsEmailsEnabled { get; internal set; }
}

/// <inheritdoc />
public class InstanceGifs : IInstanceGifs
{
    /// <inheritdoc />
    public string? Provider { get; internal set; }

    /// <inheritdoc />
    public string? DisplayName { get; internal set; }

    /// <inheritdoc />
    public bool IsAttributionRequired { get; internal set; }
}


/// <inheritdoc />
public class InstanceSSO : IInstanceSSO
{
    /// <inheritdoc />
    public bool IsEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsEnforced { get; internal set; }

    /// <inheritdoc />
    public string? DisplayName { get; internal set; }

    /// <inheritdoc />
    public string RedirectUrl { get; internal set; }
}

/// <inheritdoc />
public class InstanceRegistration : IInstanceRegistration
{
    /// <inheritdoc />
    public string Mode { get; internal set; }

    /// <inheritdoc />
    public bool IsAdminRegistrationEnabled { get; internal set; }
}

/// <inheritdoc />
public class InstanceCommunity : IInstanceCommunity
{
    /// <inheritdoc />
    public bool IsSingleCommunity { get; internal set; }

    /// <inheritdoc />
    public ulong? SingleCommunityGuildId { get; internal set; }

    /// <inheritdoc />
    public bool IsDirectMessagesDisabled { get; internal set; }
}

/// <inheritdoc />
public class InstanceServices : IInstanceServices
{
    /// <inheritdoc />
    public bool IsGifEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsYouTubeEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsBlueSkyEnabled { get; internal set; }
}

/// <inheritdoc />
public class InstanceLimits : IInstanceLimits
{
    /// <inheritdoc />
    public int Version { get; internal set; }

    /// <inheritdoc />
    public string[] Traits { get; internal set; }
}

/// <inheritdoc />
public class InstancePush : IInstancePush
{
    /// <inheritdoc />
    public string PublicVapidKey { get; internal set; }
}

/// <inheritdoc />
public class InstanceApp : IInstanceApp
{
    /// <inheritdoc />
    public InstanceAppBranding Branding { get; internal set; }

    /// <inheritdoc />
    public InstanceAppSetup Setup { get; internal set; }

    /// <inheritdoc />
    public InstanceAppLegal Legal { get; internal set; }

    /// <inheritdoc />
    public InstanceAppRegistration Registration { get; internal set; }

    IInstanceAppBranding IInstanceApp.Branding => Branding;

    IInstanceAppSetup IInstanceApp.Setup => Setup;

    IInstanceAppLegal IInstanceApp.Legal => Legal;

    IInstanceAppRegistration IInstanceApp.Registration => Registration;
}

/// <inheritdoc />
public class InstanceAppBranding : IInstanceAppBranding
{
    /// <inheritdoc />
    public string ProductName { get; internal set; }

    /// <inheritdoc />
    public string? IconUrl { get; internal set; }

    /// <inheritdoc />
    public string? SymbolUrl { get; internal set; }

    /// <inheritdoc />
    public string? LogoUrl { get; internal set; }

    /// <inheritdoc />
    public string? WordmarkUrl { get; internal set; }

    /// <inheritdoc />
    public string? FaviconUrl { get; internal set; }

    /// <inheritdoc />
    public string ThemeColor { get; internal set; }
}


/// <inheritdoc />
public class InstanceAppSetup : IInstanceAppSetup
{
    /// <inheritdoc />
    public bool IsConfigured { get; internal set; }

    /// <inheritdoc />
    public string AdminUrl { get; internal set; }
}

/// <inheritdoc />
public class InstanceAppLegal : IInstanceAppLegal
{
    /// <inheritdoc />
    public string? TermsUrl { get; internal set; }

    /// <inheritdoc />
    public string? PrivacyUrl { get; internal set; }
}

/// <inheritdoc />
public class InstanceAppRegistration : IInstanceAppRegistration
{

    /// <inheritdoc />
    public bool CollectDateOfBirth { get; internal set; }
}
