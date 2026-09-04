namespace Fluxer.Net;

/// <inheritdoc />
public class PartialGuild : Entity, IPartialGuild
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public string? IconHash { get; private set; }

    /// <inheritdoc />
    public string? BannerHash { get; private set; }

    /// <inheritdoc />
    public int? BannerWidth { get; private set; }

    /// <inheritdoc />
    public int? BannerHeight { get; private set; }

    /// <inheritdoc />
    public string? EmbedSplashHash { get; private set; }

    /// <inheritdoc />
    public int? EmbedSplashWidth { get; private set; }

    /// <inheritdoc />
    public int? EmbedSplashHeight { get; private set; }

    /// <inheritdoc />
    public string? InviteSplashHash { get; private set; }

    /// <inheritdoc />
    public int? InviteSplashWidth { get; private set; }

    /// <inheritdoc />
    public int? InviteSplashHeight { get; private set; }

    /// <inheritdoc />
    public GuildSplashCardAlignment SplashCardAligment { get; private set; }

    /// <inheritdoc />
    public GuildFeatures Features { get; private set; }

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
        data.Update(json);
        return data;
    }

    internal void Update(PartialGuildJson json)
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
        Features = GuildFeatures.FromServer(json);
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
