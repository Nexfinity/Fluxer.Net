using Newtonsoft.Json;

namespace Fluxer.Net;

public class RtcRegion : Entity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("emoji")]
    public string Emoji { get; set; }
}
