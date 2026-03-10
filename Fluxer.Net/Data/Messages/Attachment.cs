using Newtonsoft.Json;

namespace Fluxer.Net.Data.Messages;

public class Attachment
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("size")]
    public ulong Size { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    [JsonProperty("content_hash")]
    public string? ContentHash { get; set; }

    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    [JsonProperty("flags")]
    public int Flags { get; set; }

    [JsonProperty("duration")]
    public double? Duration { get; set; }

    [JsonProperty("nsfw")]
    public bool? Nsfw { get; set; }
}
