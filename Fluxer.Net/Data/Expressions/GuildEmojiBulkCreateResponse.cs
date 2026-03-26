using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildEmojiBulkCreateJson
{
    [JsonProperty("success")]
    public GuildEmojiJson[] Success { get; set; } = Array.Empty<GuildEmojiJson>();

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
