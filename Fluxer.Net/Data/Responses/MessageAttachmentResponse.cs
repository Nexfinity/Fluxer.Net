using Fluxer.Net.Data.Enums;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class MessageAttachmentResponse
{
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonRequired]
    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    [JsonRequired]
    [JsonProperty("size")]
    public ulong Size { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    [JsonRequired]
    [JsonProperty("flags")]
    public MessageAttachmentFlags Flags { get; set; }

    [JsonProperty("nsfw")]
    public bool? Nsfw { get; set; }

    /// <summary>
    /// Duration of the media in seconds
    /// </summary>
    [JsonProperty("duration")]
    public ulong? Duration { get; set; }

    [JsonProperty("waveform")]
    public string? Waveform { get; set; }

    [JsonProperty("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [JsonProperty("expired")]
    public bool? Expired { get; set; }
}
