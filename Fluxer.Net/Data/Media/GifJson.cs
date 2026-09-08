using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GifJson : IGif
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("src")]
    public string Source { get; set; }

    /// <inheritdoc />
    [JsonProperty("proxy_src")]
    public string ProxySource { get; set; }

    /// <inheritdoc />
    [JsonProperty("width")]
    public int Width { get; set; }

    /// <inheritdoc />
    [JsonProperty("height")]
    public int Height { get; set; }
}
