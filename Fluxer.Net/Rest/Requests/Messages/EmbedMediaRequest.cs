using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// A media item of a message embed, such as an image or thumbnail.
/// </summary>
public class EmbedMediaRequest
{
    /// <summary>
    /// URL of the media (image, video, etc.)
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Alt text description of the media.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }
}
