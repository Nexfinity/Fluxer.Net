using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

public class GuildEmojiResponse
{
    [JsonRequired]
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("animated")]
    public bool IsAnimated { get; set; }
}
