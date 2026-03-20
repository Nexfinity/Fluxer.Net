using Newtonsoft.Json;

namespace Fluxer.Net;

public class GifCategoryJson
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("src")]
    public string Source { get; set; }

    [JsonProperty("proxy_src")]
    public string ProxySource { get; set; }
}
