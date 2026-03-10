using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net.Rest.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L70"/>
/// </remarks>
public class RichEmbedRequest
{
    /// <summary>
    /// URL of the embed
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Title of the embed
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Color code of the embed (hex integer)
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// ISO8601 timestamp for the embed
    /// </summary>
    [JsonProperty("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Description of the embed (1-4096 characters)
    /// </summary>
    [MinLength(1)]
    [MaxLength(4096)]
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Author information
    /// </summary>
    [JsonProperty("author")]
    public RichEmbedAuthorRequest? Author { get; set; }

    /// <summary>
    /// Image to display in the embed
    /// </summary>
    [JsonProperty("image")]
    public RichEmbedMediaRequest? Image { get; set; }

    /// <summary>
    /// Thumbnail image for the embed
    /// </summary>
    [JsonProperty("thumbnail")]
    public RichEmbedMediaRequest? Thumbnail { get; set; }

    /// <summary>
    /// Footer information
    /// </summary>
    [JsonProperty("footer")]
    public RichEmbedFooterRequest? Footer { get; set; }

    /// <summary>
    /// Array of field objects (max 25)
    /// </summary>
    [MaxLength(25)]
    [JsonProperty("fields")]
    public RichEmbedFieldRequest[]? Fields { get; set; }

}
