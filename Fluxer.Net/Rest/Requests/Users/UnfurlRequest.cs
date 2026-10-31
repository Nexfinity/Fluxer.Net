using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// A url to resolve into embeds.
/// </summary>
public class UnfurlRequest
{
    /// <summary>
    /// The URL to unfurl.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }
}
