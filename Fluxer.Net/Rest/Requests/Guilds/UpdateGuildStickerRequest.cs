using Newtonsoft.Json;

namespace Fluxer.Net;

public class UpdateGuildStickerRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("tags")]
    public string[]? Tags { get; set; }
}
