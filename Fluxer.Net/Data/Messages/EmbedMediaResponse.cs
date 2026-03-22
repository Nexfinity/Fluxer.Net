using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/EmbedSchemas.tsx#L41"/>
/// </remarks>
public class EmbedMediaResponse
{
    /// <summary>
    /// The URL of the media
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// The proxied URL of the media
    /// </summary>
    [JsonProperty("proxy_url")]
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// The MIME type of the media
    /// </summary>
    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    /// <summary>
    /// The hash of the media content
    /// </summary>
    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    /// <summary>
    /// The width of the media in pixels
    /// </summary>
    [JsonProperty("width")]
    public int? Width { get; set; }

    /// <summary>
    /// The height of the media in pixels
    /// </summary>
    [JsonProperty("height")]
    public int? Height { get; set; }

    /// <summary>
    /// The description of the media
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The base64 encoded placeholder image for lazy loading
    /// </summary>
    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    /// <summary>
    /// The duration of the media in seconds
    /// </summary>
    [JsonProperty("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// The bitwise flags for this media
    /// </summary>
    [JsonProperty("flags")]
    public AttachmentFlag Flags { get; set; }
}
