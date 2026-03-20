using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildEmojiBulkCreateJson
{
    [JsonProperty("success")]
    public GuildEmojiResponse[] Success { get; set; } = Array.Empty<GuildEmojiResponse>();

    [JsonProperty("failed")]
    public GuildEmojiBulkCreateFailureItemJson[] Failed { get; set; } = Array.Empty<GuildEmojiBulkCreateFailureItemJson>();
}

public class GuildEmojiBulkCreateFailureItemJson
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonRequired]
    [JsonProperty("error")]
    public string Error { get; set; }
}
