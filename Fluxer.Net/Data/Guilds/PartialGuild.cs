namespace Fluxer.Net;

/// <inheritdoc />
public class PartialGuild : Entity, IPartialGuild
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string? IconHash { get; internal set; }

    /// <inheritdoc />
    public string? BannerHash { get; internal set; }

    /// <inheritdoc />
    public int? BannerWidth { get; internal set; }

    /// <inheritdoc />
    public int? BannerHeight { get; internal set; }

    /// <inheritdoc />
    public string? EmbedSplashHash { get; internal set; }

    /// <inheritdoc />
    public int? EmbedSplashWidth { get; internal set; }

    /// <inheritdoc />
    public int? EmbedSplashHeight { get; internal set; }

    /// <inheritdoc />
    public string? InviteSplashHash { get; internal set; }

    /// <inheritdoc />
    public int? InviteSplashWidth { get; internal set; }

    /// <inheritdoc />
    public int? InviteSplashHeight { get; internal set; }

    /// <inheritdoc />
    public GuildSplashCardAlignment SplashCardAligment { get; internal set; }

    /// <inheritdoc />
    public GuildFeatures Features { get; internal set; }

    string[]? IPartialGuild.Features => Features.Raw;

    internal PartialGuild(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a PartialGuild object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static PartialGuild Create(FluxerBaseClient client, PartialGuildJson json)
    {
        PartialGuild data = new PartialGuild(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, PartialGuildJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IconHash = json.IconHash;
        BannerHash = json.BannerHash;
        BannerWidth = json.BannerWidth;
        BannerHeight = json.BannerHeight;
        EmbedSplashHash = json.EmbedSplashHash;
        EmbedSplashWidth = json.EmbedSplashWidth;
        EmbedSplashHeight = json.EmbedSplashHeight;
        InviteSplashHash = json.InviteSplashHash;
        InviteSplashWidth = json.InviteSplashWidth;
        InviteSplashHeight = json.InviteSplashHeight;
        SplashCardAligment = json.SplashCardAligment;
        Features = GuildFeatures.FromGuild(json);
    }

    /// <inheritdoc />
    public string? GetIconUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(IconHash))
            return null;

        return $"{Client.Config.MediaUrl}/icons/{Id}/{IconHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string? GetBannerUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"{Client.Config.MediaUrl}/banners/{Id}/{BannerHash}.webp?size={size}";
    }

    /// <inheritdoc />
    public string? GetInviteSplashUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(InviteSplashHash))
            return null;

        return $"{Client.Config.MediaUrl}/splashes/{Id}/{InviteSplashHash}.webp?size={size}";
    }

    /// <inheritdoc />
    public string? GetEmbedSplashUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(EmbedSplashHash))
            return null;

        return $"{Client.Config.MediaUrl}/embed-splashes/{Id}/{EmbedSplashHash}.webp?size={size}";
    }
}
