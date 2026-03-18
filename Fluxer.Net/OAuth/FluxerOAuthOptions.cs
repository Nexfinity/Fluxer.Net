#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace Fluxer.Net.OAuth;

public class FluxerOAuthOptions : OAuthOptions
{
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

    public string? Prompt { get; set; }

    public bool EmailScope { get; set; }

    public bool GuildsScope { get; set; }

    public bool ConnectionsScope { get; set; }
}
#endif