using Newtonsoft.Json;

namespace Fluxer.Net;

public class GifFavourite : Entity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("tags")]
    public string[] Tags { get; set; }

    [JsonProperty("attachment_id")]
    public ulong AttachmentId { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("alt_text")]
    public string? AltText { get; set; }

    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("duration")]
    public int? Duration { get; set; }

    [JsonProperty("is_gifv")]
    public bool IsGifVideo { get; set; }

    [JsonProperty("klipy_slug")]
    public string? KlipySlug { get; set; }

    [JsonProperty("tenor_slug_id")]
    public string TenorSlugId { get; set; }
}
