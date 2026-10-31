using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// One format of a gif, such as "webm", "mp4", "tinygif" or "mediumwebp".
/// </summary>
public class GifMediaFormatJson
{
    /// <summary>
    /// Direct URL to this format of the media.
    /// </summary>
    [JsonProperty("src")]
    public string Source { get; set; }

    /// <summary>
    /// Proxied URL to this format of the media.
    /// </summary>
    [JsonProperty("proxy_src")]
    public string ProxySource { get; set; }

    /// <summary>
    /// Width of this format in pixels.
    /// </summary>
    [JsonProperty("width")]
    public int Width { get; set; }

    /// <summary>
    /// Height of this format in pixels.
    /// </summary>
    [JsonProperty("height")]
    public int Height { get; set; }
}

/// <summary>
/// A gif url resolved into proxy metadata.
/// </summary>
public class ResolvedGifJson
{
    /// <summary>
    /// The original gif URL.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Signed media proxy URL for the gif.
    /// </summary>
    [JsonProperty("proxy_url")]
    public string ProxyUrl { get; set; }

    /// <summary>
    /// Width of the gif in pixels, 0 when unknown.
    /// </summary>
    [JsonProperty("width")]
    public int Width { get; set; }

    /// <summary>
    /// Height of the gif in pixels, 0 when unknown.
    /// </summary>
    [JsonProperty("height")]
    public int Height { get; set; }

    /// <summary>
    /// Format name to media descriptor map. Empty when no gif provider recognizes the URL.
    /// </summary>
    [JsonProperty("media")]
    public Dictionary<string, GifMediaFormatJson>? Media { get; set; }

    /// <summary>
    /// MIME type of the primary media. An empty string means image/gif.
    /// </summary>
    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    /// <summary>
    /// Compact thumbhash placeholder for the gif.
    /// </summary>
    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }
}

/// <summary>
/// The result of resolving gif urls.
/// </summary>
public class ResolveGifUrlsJson
{
    /// <summary>
    /// The resolved gifs.
    /// </summary>
    [JsonProperty("entries")]
    public ResolvedGifJson[] Entries { get; set; }
}
