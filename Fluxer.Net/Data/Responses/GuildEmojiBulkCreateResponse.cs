using Fluxer.Net.Data.Responses;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Response;

public class GuildEmojiBulkCreateResponse
{
    [JsonPropertyName("success")]
    public GuildEmojiResponse[] Success { get; set; } = Array.Empty<GuildEmojiResponse>();
    
    [JsonPropertyName("failed")]
    public GuildEmojiBulkCreateResponseFailureItem[] Failed { get; set; } = Array.Empty<GuildEmojiBulkCreateResponseFailureItem>();
}

public class GuildEmojiBulkCreateResponseFailureItem
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonRequired]
    [JsonPropertyName("error")]
    public string Error { get; set; }
}
