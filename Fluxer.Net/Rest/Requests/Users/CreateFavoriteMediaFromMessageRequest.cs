using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Saves an attachment or embed of a message as favorite media.
/// </summary>
public class CreateFavoriteMediaFromMessageRequest
{
    /// <summary>
    /// The display name, 1 to 100 characters.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Alternative text for accessibility, up to 500 characters.
    /// </summary>
    [JsonProperty("alt_text")]
    public string? AltText { get; set; }

    /// <summary>
    /// Tags for searching the media.
    /// </summary>
    [JsonProperty("tags")]
    public IEnumerable<string>? Tags { get; set; }

    /// <summary>
    /// The attachment to save. Set this or <see cref="EmbedIndex"/>.
    /// </summary>
    [JsonProperty("attachment_id")]
    public ulong? AttachmentId { get; set; }

    /// <summary>
    /// The index of the embed to save. Set this or <see cref="AttachmentId"/>.
    /// </summary>
    [JsonProperty("embed_index")]
    public int? EmbedIndex { get; set; }
}
