using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class GuildEmojiCreateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("image")]
    public string ImageBase64 { get; set; }

    public void ImageFromStream(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        ImageBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
