using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmbedJson
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
    public EmbedAuthorJson? Author { get; set; }

    [JsonProperty("provider")]
    public EmbedProviderJson? Provider { get; set; }

    [JsonProperty("thumbnail")]
    public EmbedMediaJson? Thumbnail { get; set; }

    [JsonProperty("image")]
    public EmbedMediaJson? Image { get; set; }

    [JsonProperty("video")]
    public EmbedMediaJson? Video { get; set; }

    [JsonProperty("footer")]
    public EmbedFooterJson? Footer { get; set; }

    [JsonProperty("fields")]
    public List<EmbedFieldJson>? Fields { get; set; }

    [JsonProperty("nsfw")]
    public bool? Nsfw { get; set; }
}
