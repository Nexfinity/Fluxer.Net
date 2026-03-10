using Newtonsoft.Json;

namespace Fluxer.Net.Data.Guilds;

public class GuildEmojiBulkCreateResponse
{
    [JsonProperty("success")]
    public GuildEmojiResponse[] Success { get; set; } = Array.Empty<GuildEmojiResponse>();
    
    [JsonProperty("failed")]
    public GuildEmojiBulkCreateResponseFailureItem[] Failed { get; set; } = Array.Empty<GuildEmojiBulkCreateResponseFailureItem>();
}

public class GuildEmojiBulkCreateResponseFailureItem
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonRequired]
    [JsonProperty("error")]
    public string Error { get; set; }
}
