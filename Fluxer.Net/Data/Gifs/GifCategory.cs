using Newtonsoft.Json;

namespace Fluxer.Net;

public class GifCategory : Entity
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("src")]
    public string Source { get; set; }

    [JsonProperty("proxy_src")]
    public string ProxySource { get; set; }
}
