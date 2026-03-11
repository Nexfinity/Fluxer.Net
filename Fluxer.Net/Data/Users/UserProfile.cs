using Newtonsoft.Json;

namespace Fluxer.Net;

public class UserProfile : Entity
{
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    [JsonProperty("banner")]
    public string? BannerHash { get; set; }
}
