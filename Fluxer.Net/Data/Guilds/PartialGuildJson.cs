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
    public string? InviteSplashHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash_width")]
    public int? InviteSplashWidth { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash_height")]
    public int? InviteSplashHeight { get; set; }

    /// <inheritdoc />
    [JsonProperty("splash_card_alignment")]
    public GuildSplashCardAlignment SplashCardAligment { get; set; }

    /// <inheritdoc />
    [JsonProperty("features")]
    public string[]? Features { get; set; }

    /// <inheritdoc />
    public string? GetIconUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(IconHash))
            return null;

        return $"https://fluxerusercontent.com/icons/{Id}/{IconHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string? GetBannerUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"https://fluxerusercontent.com/banners/{Id}/{BannerHash}.webp?size={size}";
    }

    /// <inheritdoc />
    public string? GetInviteSplashUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(InviteSplashHash))
            return null;

        return $"https://fluxerusercontent.com/splashes/{Id}/{InviteSplashHash}.webp?size={size}";
    }

    /// <inheritdoc />
    public string? GetEmbedSplashUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(EmbedSplashHash))
            return null;

        return $"https://fluxerusercontent.com/embed-splashes/{Id}/{EmbedSplashHash}.webp?size={size}";
    }
}
