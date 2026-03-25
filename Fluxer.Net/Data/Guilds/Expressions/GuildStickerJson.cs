using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildStickerJson : IGuildSticker
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("tags")]
    public List<string>? Tags { get; set; }

    /// <inheritdoc />
    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public User? Creator { get; set; }
}
