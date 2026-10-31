using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Gif urls to resolve into entries with proxy metadata.
/// </summary>
public class ResolveGifUrlsRequest
{
    /// <summary>
    /// The gif URLs, between 1 and 200.
    /// </summary>
    [JsonProperty("urls")]
    public IEnumerable<string> Urls { get; set; }
}
