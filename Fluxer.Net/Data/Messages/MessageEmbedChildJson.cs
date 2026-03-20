using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/EmbedSchemas.tsx#L91"/>
/// </remarks>
public class MessageEmbedChildJson
{
    /// <summary>
    /// The type of embed (e.g., rich, image, video, gifv, article, link)
    /// </summary>
    /// <remarks>
    /// Values: <see href="https://github.com/fluxerapp/fluxer/blob/d843d6f3f8a0bd850673ba55036354093977109f/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L119"/>
    /// </remarks>
    [JsonRequired]
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// The URL of the embed
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <summary>
    /// The title of the embed
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The color code of the embed sidebar
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// The ISO 8601 timestamp of the embed content
    /// </summary>
    [JsonProperty("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// The description of the embed
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The author information of the embed
    /// </summary>
    [JsonProperty("author")]
    public EmbedAuthorResponse? Author { get; set; }

    /// <summary>
    /// The image of the embed
    /// </summary>
    [JsonProperty("image")]
    public EmbedMediaResponse? Image { get; set; }

    /// <summary>
    /// The thumbnail of the embed
    /// </summary>
    [JsonProperty("thumbnail")]
    public EmbedMediaResponse? Thumbnail { get; set; }

    /// <summary>
    /// The footer of the embed
    /// </summary>
    [JsonProperty("footer")]
    public EmbedFooterResponse? Footer { get; set; }

    /// <summary>
    /// The fields of the embed
    /// </summary>
    [MaxLength(25)]
    [JsonProperty("fields")]
    public EmbedFieldResponse[]? Fields { get; set; }

    /// <summary>
    /// The provider of the embed (e.g., YouTube, Twitter)
    /// </summary>
    [JsonProperty("provider")]
    public EmbedAuthorResponse? Provider { get; set; }

    /// <summary>
    /// The video of the embed
    /// </summary>
    [JsonProperty("video")]
    public EmbedMediaResponse? Video { get; set; }

    /// <summary>
    /// The audio of the embed
    /// </summary>
    [JsonProperty("audio")]
    public EmbedMediaResponse? Audio { get; set; }

    /// <summary>
    /// Whether the embed is flagged as NSFW
    /// </summary>
    [JsonProperty("nsfw")]
    public bool? Nsfw { get; set; }
}
