using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Fluxer.Net;

public class CreateGuildRequest
{
    [JsonProperty("empty_features")]
    public bool? EmptyFeatures { get; set; }

    [JsonProperty("icon")]
    public string? IconBase64 { get; set; }
    
    [MinLength(1)]
    [MaxLength(100)]
    [JsonProperty("name")]
    public string Name { get; set; }

    public void IconFromStream(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        IconBase64 = Convert.ToBase64String(ms.ToArray());
    }
}
