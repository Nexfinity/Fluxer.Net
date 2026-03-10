using Newtonsoft.Json;

namespace Fluxer.Net.Data.Guilds;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildEmojiSchemas.tsx#L41"/>
/// </remarks>
public class GuildStickerResponse : Entity
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("tags")]
    public HashSet<string> Tags { get; set; } = new();

    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }
}
