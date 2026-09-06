using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class UpdateGuildEmojiRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }
}
