using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

public class GuildEmojiUpdateRequest
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
