using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildEmojiCreateRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("image")]
    public string ImageBase64 { get; set; }

    public void ImageFromStream(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        ImageBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
