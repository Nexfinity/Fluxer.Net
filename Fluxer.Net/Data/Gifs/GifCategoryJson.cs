using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GifCategoryJson : IGifCategory
{
    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("src")]
    public string Source { get; set; }

    /// <inheritdoc />
    [JsonProperty("proxy_src")]
    public string ProxySource { get; set; }
}
