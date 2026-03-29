using Newtonsoft.Json;

namespace Fluxer.Net;

public class UpdateGuildEmojiRequest
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
