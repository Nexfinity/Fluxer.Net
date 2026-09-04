namespace Fluxer.Net;

/// <inheritdoc />
public class UserProfile : Entity, IUserProfile
{
    /// <summary>
    /// User's id for the profile.
    /// </summary>
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public string? Bio { get; private set; }

    /// <inheritdoc />
    public string? Pronouns { get; private set; }

    /// <inheritdoc />
    public int? AccentColor { get; private set; }

    /// <inheritdoc />
    public string? BannerHash { get; private set; }

    /// <inheritdoc />
    public int? BannerColor { get; private set; }

    /// <summary>
    /// Get the user's banner.
    /// </summary>
    public string? GetBannerUrl(int size = 600)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"{Client.Config.MediaUrl}/banners/{UserId}/{BannerHash}.png?size={size}";
    }

    internal UserProfile(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a UserProfile object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static UserProfile Create(FluxerBaseClient client, UserProfileJson json, ulong userId)
    {
        UserProfile data = new UserProfile(client)
        {
            UserId = userId
        };
        data.Update(json);
        return data;
    }

    internal void Update(UserProfileJson json)
    {
        Bio = json.Bio;
        Pronouns = json.Pronouns;
        AccentColor = json.AccentColor;
        BannerHash = json.BannerHash;
        BannerColor = json.BannerColor;
    }
}
