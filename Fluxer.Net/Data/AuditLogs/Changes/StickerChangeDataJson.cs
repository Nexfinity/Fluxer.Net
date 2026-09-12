using Newtonsoft.Json;

namespace Fluxer.Net;

public class StickerChangeDataJson : IAuditLogData
{
    [JsonProperty("sticker_id")]
    public ulong StickerId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }

    [JsonProperty("creator_id")]
    public ulong CreatorId { get; set; }
}
