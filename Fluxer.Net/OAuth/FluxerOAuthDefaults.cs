namespace Fluxer.Net.OAuth;

public static class FluxerOAuthDefaults
{
    public static readonly string DisplayName = "Fluxer";

    public static readonly string AuthenticationScheme = "Fluxer";

    public static readonly string Issuer = "Fluxer";

    public static readonly string CallbackPath = "/signin-fluxer";

    public static readonly string AuthorizationEndpoint = "https://api.fluxer.app/v1/oauth2/authorize";

    public static readonly string TokenEndpoint = "https://api.fluxer.app/v1/oauth2/token";

    public static readonly string UserInformationEndpoint = "https://api.fluxer.app/v1/oauth2/userinfo";
}
