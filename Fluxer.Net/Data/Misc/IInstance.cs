namespace Fluxer.Net;

public interface IInstance
{
    int ApiVersion { get; }

    IInstanceEndpoints Endpoints { get; }

    IInstanceCaptcha Captcha { get; }

    IInstanceFeatures Features { get; }

    IInstanceGifs Gifs { get; }

    IInstanceSSO SSO { get; }

    IInstanceRegistration Registration { get; }

    IInstanceCommunity Community { get; }

    IInstanceServices Services { get; }

    IInstanceLimits Limits { get; }

    IInstancePush Push { get; }

    IInstanceApp App { get; }
}
public interface IInstanceEndpoints
{
    string Api { get; }

    string ApiClient { get; }

    string ApiPublic { get; }

    string Gateway { get; }

    string Media { get; }

    string Static { get; }

    string Marketing { get; }

    string Admin { get; }

    string Invite { get; }

    string Gift { get; }

    string WebApp { get; }
}
public interface IInstanceCaptcha
{
    string? Provider { get; }

    string? HCaptchaSiteKey { get; }

    string? TurnstileSiteKey { get; }
}
public interface IInstanceFeatures
{
    bool IsVoiceEnabled { get; }

    bool IsStripeEnabled { get; }

    bool IsSelfHosted { get; }

    bool IsPresignedAttachmentUploads { get; }

    bool IsEmailsEnabled { get; }
}
public interface IInstanceGifs
{
    string? Provider { get; }

    string? DisplayName { get; }

    bool IsAttributionRequired { get; }
}
public interface IInstanceSSO
{
    bool IsEnabled { get; }

    bool IsEnforced { get; }

    string? DisplayName { get; }

    string RedirectUrl { get; }
}
public interface IInstanceRegistration
{
    string Mode { get; }

    bool IsAdminRegistrationEnabled { get; }
}
public interface IInstanceCommunity
{
    bool IsSingleCommunity { get; }

    ulong? SingleCommunityGuildId { get; }

    bool IsDirectMessagesDisabled { get; }
}
public interface IInstanceServices
{
    bool IsGifEnabled { get; }

    bool IsYouTubeEnabled { get; }

    bool IsBlueSkyEnabled { get; }
}
public interface IInstanceLimits
{
    int Version { get; }

    string[] Traits { get; }
}
public interface IInstancePush
{
    string PublicVapidKey { get; }
}
public interface IInstanceApp
{
    IInstanceAppBranding Branding { get; }

    IInstanceAppSetup Setup { get; }

    IInstanceAppLegal Legal { get; }

    IInstanceAppRegistration Registration { get; }
}
public interface IInstanceAppBranding
{
    string ProductName { get; }

    string? IconUrl { get; }

    string? SymbolUrl { get; }

    string? LogoUrl { get; }

    string? WordmarkUrl { get; }

    string? FaviconUrl { get; }

    string ThemeColor { get; }
}
public interface IInstanceAppSetup
{
    bool IsConfigured { get; }

    string AdminUrl { get; }
}
public interface IInstanceAppLegal
{
    string? TermsUrl { get; }

    string? PrivacyUrl { get; }
}
public interface IInstanceAppRegistration
{
    bool CollectDateOfBirth { get; }
}