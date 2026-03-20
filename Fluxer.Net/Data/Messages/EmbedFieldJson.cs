using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmbedFieldJson
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("inline")]
    public bool Inline { get; set; }
}
