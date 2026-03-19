using Newtonsoft.Json;

namespace Fluxer.Net;

public class PartialGuildJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    [JsonProperty("banner_width")]
    public int? BannerWidth { get; set; }

    [JsonProperty("banner_height")]
    public int? BannerHeight { get; set; }

    [JsonProperty("embed_splash")]
    public string? EmbedSplashHash { get; set; }

    [JsonProperty("embed_splash_width")]
    public int? EmbedSplashWidth { get; set; }

    [JsonProperty("embed_splash_height")]
    public int? EmbedSplashHeight { get; set; }

    [JsonProperty("splash")]
    public string? SplashHash { get; set; }

    [JsonProperty("splash_width")]
    public int? SplashWidth { get; set; }

    [JsonProperty("splash_height")]
    public int? SplashHeight { get; set; }

    [JsonProperty("splash_card_alignment")]
    public GuildSplashCardAlignment SplashCardAligment { get; set; }

    [JsonProperty("features")]
    public HashSet<string>? Features { get; set; }
}
