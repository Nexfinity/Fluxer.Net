using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildEmojiUpdateRequest
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
