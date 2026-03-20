using Newtonsoft.Json;

namespace Fluxer.Net;

public class GifFeaturedJson
{
    [JsonProperty("gifs")]
    public GifJson[] Gifs { get; set; }

    [JsonProperty("categories")]
    public GifCategoryJson[] Categories { get; set; }
}
