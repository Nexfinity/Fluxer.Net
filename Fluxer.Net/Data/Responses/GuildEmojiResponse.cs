using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class GuildEmojiResponse
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }
}
