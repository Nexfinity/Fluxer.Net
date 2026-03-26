using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class UserProfileJson : IUserProfile
{
    /// <inheritdoc />
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <inheritdoc />
    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    /// <inheritdoc />
    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner_color")]
    public int? BannerColor { get; set; }
}
