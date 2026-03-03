using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildRequestSchemas.tsx#L210"/>
/// </remarks>
public class GuildStickerCreateRequest
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
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        ImageBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
