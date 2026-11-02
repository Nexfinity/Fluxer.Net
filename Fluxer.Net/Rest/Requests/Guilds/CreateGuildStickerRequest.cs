using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a sticker in a guild.
/// </summary>
public class CreateGuildStickerRequest
{
    /// <summary>
    /// The name of the sticker.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The description of the sticker.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The tags that describe the sticker.
    /// </summary>
    [JsonProperty("tags")]
    public string[]? Tags { get; set; }

    /// <summary>
    /// Base64 image data for the sticker.
    /// </summary>
    [JsonProperty("image")]
    public string ImageBase64 { get; set; }

    /// <summary>
    /// Sets <see cref="ImageBase64"/> from an image stream.
    /// </summary>
    public void ImageFromStream(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        ImageBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
