using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

public class RichEmbedMediaRequest
{
    /// <summary>
    /// URL of the media (image, video, etc.)
    /// </summary>
    [JsonRequired]
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Alt text description of the media
    /// </summary>
    [MinLength(1)]
    [MaxLength(4096)]
    [JsonProperty("description")]
    public string? Description { get; set; }
}
