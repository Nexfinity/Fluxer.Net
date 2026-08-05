namespace Fluxer.Net;

/// <inheritdoc />
public class UserProfile : Entity, IUserProfile
{
    /// <summary>
    /// User's id for the profile.
    /// </summary>
    public ulong UserId { get; internal set; }

    /// <inheritdoc />
    public string? Bio { get; internal set; }

    /// <inheritdoc />
    public string? Pronouns { get; internal set; }

    /// <inheritdoc />
    public int? AccentColor { get; internal set; }

    /// <inheritdoc />
    public string? BannerHash { get; internal set; }

    /// <inheritdoc />
    public int? BannerColor { get; internal set; }

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
        UserProfile data = new UserProfile(client);
        data.UserId = userId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, UserProfileJson json)
    {
        Bio = json.Bio;
        Pronouns = json.Pronouns;
        AccentColor = json.AccentColor;
        BannerHash = json.BannerHash;
        BannerColor = json.BannerColor;
    }
}
