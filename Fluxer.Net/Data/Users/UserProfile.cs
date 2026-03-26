namespace Fluxer.Net;

/// <inheritdoc />
public class UserProfile : Entity, IUserProfile
{
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

    internal UserProfile(FluxerBaseClient client) : base(client)
    {

    }

    public static UserProfile Create(FluxerBaseClient client, UserProfileJson json)
    {
        var data = new UserProfile(client);
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
