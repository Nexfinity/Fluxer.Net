using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildStickerJson : StickerJson, IGuildSticker
{
    /// <inheritdoc />
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("tags")]
    public List<string>? Tags { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson? Creator { get; set; }

    IUser? IGuildSticker.Creator => Creator;
}
