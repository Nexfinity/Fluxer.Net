using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class BulkCreateGuildStickersRequest
{
    [JsonProperty("stickers")]
    public CreateGuildStickerRequest[] Stickers { get; set; } = Array.Empty<CreateGuildStickerRequest>();
}
