using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The featured GIFs and GIF categories, as the API sends them.
/// </summary>
public class GifFeaturedJson
{
    /// <summary>
    /// The featured GIFs.
    /// </summary>
    [JsonProperty("gifs")]
    public GifJson[] Gifs { get; set; }

    /// <summary>
    /// The GIF categories.
    /// </summary>
    [JsonProperty("categories")]
    public GifCategoryJson[] Categories { get; set; }
}
