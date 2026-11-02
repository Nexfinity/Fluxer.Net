using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating an emoji in a guild.
/// </summary>
public class CreateGuildEmojiRequest
{
    /// <summary>
    /// The name of the emoji.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Base64 image data for the emoji.
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
