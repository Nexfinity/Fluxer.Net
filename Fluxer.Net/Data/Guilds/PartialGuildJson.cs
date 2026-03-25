using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class PartialGuildJson : IPartialGuild
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner_width")]
    public int? BannerWidth { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner_height")]
    public int? BannerHeight { get; set; }

    /// <inheritdoc />
    [JsonProperty("embed_splash")]
    public string? EmbedSplashHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("embed_splash_width")]
    public int? EmbedSplashWidth { get; set; }

    /// <inheritdoc />
    [JsonProperty("embed_splash_height")]
    public int? EmbedSplashHeight { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash")]
    public string? SplashHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash_width")]
    public int? SplashWidth { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash_height")]
    public int? SplashHeight { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash_card_alignment")]
    public GuildSplashCardAlignment SplashCardAligment { get; set; }

    /// <inheritdoc />
    [JsonProperty("features")]
    public string[]? Features { get; set; }
}
