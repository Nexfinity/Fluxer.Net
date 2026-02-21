using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class GuildEmojiUpdateRequest
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
