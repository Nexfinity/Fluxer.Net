using System.Security.Claims;

namespace Fluxer.Net.OAuth;

public class FluxerOAuthClaims
{
    public FluxerOAuthClaims(ClaimsPrincipal principal)
    {
        foreach (var c in principal.Claims)
        {
            switch (c.Type)
            {
                case ClaimTypes.NameIdentifier:
                    Id = c.Value;
                    break;
                case ClaimTypes.Name:
                    Username = c.Value;
                    break;
                case ClaimTypes.Email:
                    Email = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.Discriminator:
                    Discriminator = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.DisplayName:
                    DisplayName = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.AvatarHash:
                    AvatarHash = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.Verified:
                    Verified = bool.Parse(c.Value);
                    break;
                case FluxerOAuthConstants.Claims.Flags:
                    Flags = ulong.Parse(c.Value);
                    break;
            }
        }
    }
    public string Id { get; }
    public string Username { get; }
    public string Discriminator { get; }
    public string DisplayName { get; }
    public string? Email { get; }
    public string? AvatarHash { get; }
    public bool? Verified { get; }
    public ulong Flags { get; }
}
