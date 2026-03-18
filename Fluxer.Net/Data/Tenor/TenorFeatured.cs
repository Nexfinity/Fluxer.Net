using Newtonsoft.Json;

namespace Fluxer.Net;

public class TenorFeatured : Entity
{
    [JsonProperty("gifs")]
    public TenorGif[] Gifs { get; set; }

    [JsonProperty("categories")]
    public TenorCategory[] Categories { get; set; }
}
