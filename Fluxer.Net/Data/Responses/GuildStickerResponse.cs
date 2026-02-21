using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildEmojiSchemas.tsx#L41"/>
/// </remarks>
public class GuildStickerResponse
{
    [JsonRequired]
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("tags")]
    public HashSet<string> Tags { get; set; } = new();

    [JsonPropertyName("animated")]
    public bool IsAnimated { get; set; }
}
