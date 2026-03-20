using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageStickerJson
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("aminated")]
    public bool Animated { get; set; }
}
