using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class UpdateGuildEmojiRequest
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
