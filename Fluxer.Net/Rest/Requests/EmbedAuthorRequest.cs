using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

public class EmbedAuthorRequest
{
    /// <summary>
    /// Name of the embed author.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// URL to link from the author name/
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <summary>
    /// URL of the author icon.
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }
}
