using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildEmojiSchemas.tsx#L77C14-L77C44"/>
/// </remarks>
public class GuildStickerBulkCreateJson
{
    [JsonProperty("success")]
    public GuildStickerResponse[] Success { get; set; }

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