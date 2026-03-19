using Newtonsoft.Json;

namespace Fluxer.Net;

public class GifFeatured : Entity
{
    [JsonProperty("gifs")]
    public Gif[] Gifs { get; set; }

    [JsonProperty("categories")]
    public GifCategory[] Categories { get; set; }
}
