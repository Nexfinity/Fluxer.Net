using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class FavouriteGifJson : IFavouriteGif
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("tags")]
    public string[] Tags { get; set; }

    /// <inheritdoc />
    [JsonProperty("attachment_id")]
    public ulong AttachmentId { get; set; }

    /// <inheritdoc />
    [JsonProperty("filename")]
    public string Filename { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    /// <inheritdoc />
    [JsonProperty("size")]
    public int Size { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("alt_text")]
    public string? AltText { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("width")]
    public int? Width { get; set; }

    /// <inheritdoc />
    [JsonProperty("height")]
    public int? Height { get; set; }

    /// <inheritdoc />
    [JsonProperty("duration")]
    public int? Duration { get; set; }

    /// <inheritdoc />
    [JsonProperty("is_gifv")]
    public bool IsGifVideo { get; set; }

    /// <inheritdoc />
    [JsonProperty("klipy_slug")]
    public string? KlipySlug { get; set; }

    /// <inheritdoc />
    [JsonProperty("tenor_slug_id")]
    public string? TenorSlugId { get; set; }
}
