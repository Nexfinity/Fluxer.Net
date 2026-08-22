using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class AttachmentJson : IAttachment
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonProperty("filename")]
    public string Filename { get; set; }

    /// <inheritdoc />
    [JsonProperty("size")]
    public ulong Size { get; set; }

    /// <inheritdoc />
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <inheritdoc />
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <inheritdoc />
    [JsonProperty("width")]
    public int? Width { get; set; }

    /// <inheritdoc />
    [JsonProperty("height")]
    public int? Height { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    /// <inheritdoc />
    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public AttachmentFlag Flags { get; set; }

    /// <inheritdoc />
    [JsonProperty("duration")]
    public ulong? Duration { get; set; }

    /// <inheritdoc />
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <inheritdoc />
    [JsonProperty("proxy_url")]
    public string? ProxyUrl { get; set; }

    /// <inheritdoc />
    [JsonProperty("nsfw")]
    public bool? IsNsfw { get; set; }

    /// <inheritdoc />
    [JsonProperty("waveform")]
    public string? Waveform { get; set; }

    /// <inheritdoc />
    [JsonProperty("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("expired")]
    public bool? IsExpired { get; set; }
}
