using Newtonsoft.Json;

namespace Fluxer.Net;

public class CreateGuildStickerRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("tags")]
    public string[]? Tags { get; set; }

    [JsonProperty("image")]
    public string ImageBase64 { get; set; }

    public void ImageFromStream(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        ImageBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
