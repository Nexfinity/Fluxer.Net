using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmojiChangeDataJson : IAuditLogData
{
    [JsonProperty("emoji_id")]
    public ulong EmojiId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }

    [JsonProperty("creator_id")]
    public ulong CreatorId { get; set; }
}
