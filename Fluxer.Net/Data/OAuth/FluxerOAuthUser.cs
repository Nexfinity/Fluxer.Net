using Fluxer.Net.OAuth;
using System.Security.Claims;

namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthUser : User, IFluxerOAuthUser
{
    internal FluxerOAuthUser(FluxerBaseClient client, ClaimsPrincipal principal) : base(client)
    {
        if (principal == null)
            return;

        foreach (Claim c in principal.Claims)
        {
            switch (c.Type)
            {
                case ClaimTypes.NameIdentifier:
                    Id = ulong.Parse(c.Value);
                    CreatedAt = SnowflakeUtils.FromSnowflake(Id);
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
                    IsVerified = bool.Parse(c.Value);
                    break;
                case FluxerOAuthConstants.Claims.Flags:
                    Flags = (UserFlags)ulong.Parse(c.Value);
                    break;
            }
        }
    }

    /// <summary>
    /// Create a FluxerOAuthUser object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static FluxerOAuthUser Create(FluxerBaseClient client, FluxerOAuthUserJson json)
    {
        FluxerOAuthUser data = new FluxerOAuthUser(client, null);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, FluxerOAuthUserJson json)
    {
        base.Update(json);
        Email = json.Email;
        IsVerified = json.IsVerified;
    }


    /// <inheritdoc />
    public string? Email { get; internal set; }

    /// <inheritdoc />
    public bool? IsVerified { get; internal set; }
}
