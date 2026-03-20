using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmbedProviderJson
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }
}
