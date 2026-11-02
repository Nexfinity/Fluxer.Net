namespace Fluxer.Net.OAuth;

/// <summary>
/// The defaults for signing in with Fluxer, which point at the main instance.
/// </summary>
public static class FluxerOAuthDefaults
{
    /// <summary>
    /// The display name of the Fluxer sign-in.
    /// </summary>
    public static readonly string DisplayName = "Fluxer";

    /// <summary>
    /// The name of the Fluxer authentication scheme.
    /// </summary>
    public static readonly string AuthenticationScheme = "Fluxer";

    /// <summary>
    /// The claims issuer of Fluxer sign-ins.
    /// </summary>
    public static readonly string Issuer = "Fluxer";

    /// <summary>
    /// The path that Fluxer redirects to after the user signs in.
    /// </summary>
    public static readonly string CallbackPath = "/signin-fluxer";

    /// <summary>
    /// The OAuth authorization endpoint of the main instance.
    /// </summary>
    public static readonly string AuthorizationEndpoint = "https://api.fluxer.app/v1/oauth2/authorize";

    /// <summary>
    /// The OAuth token endpoint of the main instance.
    /// </summary>
    public static readonly string TokenEndpoint = "https://api.fluxer.app/v1/oauth2/token";

    /// <summary>
    /// The OAuth user information endpoint of the main instance.
    /// </summary>
    public static readonly string UserInformationEndpoint = "https://api.fluxer.app/v1/oauth2/userinfo";
}
