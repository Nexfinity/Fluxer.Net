using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

public class GuildEmojiUpdateRequest
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
