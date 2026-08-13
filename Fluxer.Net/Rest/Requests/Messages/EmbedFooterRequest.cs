using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class EmbedFooterRequest
{
    /// <summary>
    /// Footer text (1-2048 characters)
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }

    /// <summary>
    /// URL of the footer icon.
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }
}
