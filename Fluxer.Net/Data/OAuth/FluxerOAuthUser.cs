using Fluxer.Net.OAuth;
using System.Security.Claims;

namespace Fluxer.Net;

/// <inheritdoc />
public class FluxerOAuthUser : User, IFluxerOAuthUser
{
    internal FluxerOAuthUser(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a FluxerOAuthUser object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static FluxerOAuthUser Create(FluxerBaseClient client, FluxerOAuthUserJson json)
    {
        FluxerOAuthUser data = new FluxerOAuthUser(client);
        data.Update(json);
        return data;
    }

    public static FluxerOAuthUser Create(FluxerBaseClient client, ClaimsPrincipal principal)
    {
        FluxerOAuthUser data = new FluxerOAuthUser(client);
        foreach (Claim c in principal.Claims)
        {
            switch (c.Type)
            {
                case ClaimTypes.NameIdentifier:
                    data.Id = ulong.Parse(c.Value);
                    data.CreatedAt = SnowflakeUtils.FromSnowflake(data.Id);
                    break;
                case ClaimTypes.Name:
                    data.Username = c.Value;
                    break;
                case ClaimTypes.Email:
                    data.Email = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.Discriminator:
                    data.Discriminator = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.DisplayName:
                    data.DisplayName = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.AvatarHash:
                    data.AvatarHash = c.Value;
                    break;
                case FluxerOAuthConstants.Claims.Verified:
                    data.IsVerified = bool.Parse(c.Value);
                    break;
                case FluxerOAuthConstants.Claims.Flags:
                    data.Flags = (UserFlags)ulong.Parse(c.Value);
                    break;
            }
        }
        return data;
    }

    internal void Update(FluxerOAuthUserJson json)
    {
        base.Update(json);
        Email = json.Email;
        IsVerified = json.IsVerified;
    }


    /// <inheritdoc />
    public string? Email { get; private set; }

    /// <inheritdoc />
    public bool? IsVerified { get; private set; }
}
