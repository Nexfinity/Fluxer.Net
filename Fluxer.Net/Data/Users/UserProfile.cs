namespace Fluxer.Net;

/// <summary>
/// The profile of a user, with the bio, pronouns and banner.
/// </summary>
public class UserProfile : Entity, IUserProfile
{
    /// <summary>
    /// The id of the user the profile belongs to.
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
    /// Gets the URL of the user's banner, or null when the user has none.
    /// </summary>
    /// <param name="size">The requested image size.</param>
    public string? GetBannerUrl(int size = 600)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return CdnUrl.Png($"{Client.Config.MediaUrl}/banners/{UserId}/{BannerHash}", BannerHash, size);
    }

    internal UserProfile(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a profile from its JSON data.
    /// </summary>
    /// <param name="client">The client the profile belongs to.</param>
    /// <param name="json">The profile data.</param>
    /// <param name="userId">The id of the user the profile belongs to.</param>
    /// <returns>The profile.</returns>
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
