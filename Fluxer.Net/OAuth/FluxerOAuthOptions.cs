#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace Fluxer.Net.OAuth;

/// <summary>
/// The options of the Fluxer sign-in handler.
/// </summary>
public class FluxerOAuthOptions : OAuthOptions
{
    /// <summary>
    /// Creates options that use the main Fluxer instance, map the user's claims and request the identify scope.
    /// </summary>
    public FluxerOAuthOptions()
    {
        ClaimsIssuer = FluxerOAuthDefaults.Issuer;
        CallbackPath = FluxerOAuthDefaults.CallbackPath;
        AuthorizationEndpoint = FluxerOAuthDefaults.AuthorizationEndpoint;
        TokenEndpoint = FluxerOAuthDefaults.TokenEndpoint;
        UserInformationEndpoint = FluxerOAuthDefaults.UserInformationEndpoint;

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
        ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        ClaimActions.MapJsonKey(FluxerOAuthConstants.Claims.AvatarHash, "avatar");
        ClaimActions.MapJsonKey(FluxerOAuthConstants.Claims.Discriminator, "discriminator");
        ClaimActions.MapJsonKey(FluxerOAuthConstants.Claims.DisplayName, "global_name");
        ClaimActions.MapJsonKey(FluxerOAuthConstants.Claims.Flags, "flags");
        ClaimActions.MapJsonKey(FluxerOAuthConstants.Claims.Verified, "verified");

        Scope.Add("identify");
    }

    /// <summary>
    /// The prompt value sent with the authorization request. Not sent when null or empty.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Points the sign in at an instance's API, such as <c>https://api.example.org</c> or
    /// <c>https://api.example.org/v1</c>.
    /// </summary>
    /// <param name="apiUrl">The instance's public API URL.</param>
    /// <returns>These options.</returns>
    public FluxerOAuthOptions UseInstance(string apiUrl)
    {
        string api = apiUrl.Trim().TrimEnd('/');
        if (!System.Text.RegularExpressions.Regex.IsMatch(api, @"/v\d+$"))
            api += "/v1";
        AuthorizationEndpoint = $"{api}/oauth2/authorize";
        TokenEndpoint = $"{api}/oauth2/token";
        UserInformationEndpoint = $"{api}/oauth2/userinfo";
        return this;
    }

    /// <summary>
    /// Whether to request the email scope.
    /// </summary>
    public bool EmailScope { get; set; }

    /// <summary>
    /// Whether to request the guilds scope.
    /// </summary>
    public bool GuildsScope { get; set; }

    /// <summary>
    /// Whether to request the connections scope.
    /// </summary>
    public bool ConnectionsScope { get; set; }
}
#endif