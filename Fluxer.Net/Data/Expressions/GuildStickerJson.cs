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

    /// <inheritdoc />
    public string? GetStickerUrl(int size = 320)
    {
        return $"https://fluxerusercontent.com/stickers/{Id}.webp?size={size}";
    }

    IUser? IGuildSticker.Creator => Creator;
}
