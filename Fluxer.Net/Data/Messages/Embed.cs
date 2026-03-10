using Newtonsoft.Json;

namespace Fluxer.Net;

public class Embed
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonProperty("color")]
    public int? Color { get; set; }

    [JsonProperty("author")]
    public EmbedAuthor? Author { get; set; }

    [JsonProperty("provider")]
    public EmbedProvider? Provider { get; set; }

    [JsonProperty("thumbnail")]
    public EmbedMedia? Thumbnail { get; set; }

    [JsonProperty("image")]
    public EmbedMedia? Image { get; set; }

    [JsonProperty("video")]
    public EmbedMedia? Video { get; set; }

    [JsonProperty("footer")]
    public EmbedFooter? Footer { get; set; }

    [JsonProperty("fields")]
    public List<EmbedField>? Fields { get; set; }

    [JsonProperty("nsfw")]
    public bool? Nsfw { get; set; }
}
