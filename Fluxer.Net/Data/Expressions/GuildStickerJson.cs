using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildStickerJson : StickerJson
{
    /// <inheritdoc />
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("tags")]
    public List<string>? Tags { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public User? Creator { get; set; }
}
