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
    public string? SplashHash { get; internal set; }

    /// <inheritdoc />
    public int? SplashWidth { get; internal set; }

    /// <inheritdoc />
    public int? SplashHeight { get; internal set; }

    /// <inheritdoc />
    public GuildSplashCardAlignment SplashCardAligment { get; internal set; }

    /// <inheritdoc />
    public GuildFeatures Features { get; internal set; }

    string[]? IPartialGuild.Features => Features.Raw;

    internal PartialGuild(FluxerBaseClient client) : base(client)
    {

    }

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
        SplashHash = json.SplashHash;
        SplashWidth = json.SplashWidth;
        SplashHeight = json.SplashHeight;
        SplashCardAligment = json.SplashCardAligment;
        Features = GuildFeatures.FromGuild(json);
    }
}
