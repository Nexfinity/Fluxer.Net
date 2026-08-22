using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class EmbedRequest
{
    [JsonProperty("type")]
    public string? Type { get; internal set; } = "rich";

    /// <summary>
    /// URL of the embed.
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Title of the embed.
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Color code of the embed (hex integer)
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// ISO8601 timestamp for the embed.
    /// </summary>
    [JsonProperty("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Description of the embed (1-4096 characters)
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Author information.
    /// </summary>
    [JsonProperty("author")]
    public EmbedAuthorRequest? Author { get; set; }

    /// <summary>
    /// Image to display in the embed.
    /// </summary>
    [JsonProperty("image")]
    public EmbedMediaRequest? Image { get; set; }

    /// <summary>
    /// Thumbnail image for the embed.
    /// </summary>
    [JsonProperty("thumbnail")]
    public EmbedMediaRequest? Thumbnail { get; set; }

    /// <summary>
    /// Footer information.
    /// </summary>
    [JsonProperty("footer")]
    public EmbedFooterRequest? Footer { get; set; }

    /// <summary>
    /// Array of field objects (max 25)
    /// </summary>
    [JsonProperty("fields")]
    public EmbedFieldRequest[]? Fields { get; set; }

}
