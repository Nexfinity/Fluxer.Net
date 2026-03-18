using Newtonsoft.Json;

namespace Fluxer.Net;

public class TenorGif : Entity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("src")]
    public string Source { get; set; }

    [JsonProperty("proxy_src")]
    public string ProxySource { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}
