using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildStickerBulkCreateJson
{
    [JsonProperty("success")]
    public GuildStickerJson[] Success { get; set; }

    [JsonProperty("failed")]
    public GuildStickerBulkCreateFailureItemJson[] Failed { get; set; } = Array.Empty<GuildStickerBulkCreateFailureItemJson>();
}

public class GuildStickerBulkCreateFailureItemJson
{
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonRequired]
    [JsonProperty("error")]
    public string Error { get; set; }
}