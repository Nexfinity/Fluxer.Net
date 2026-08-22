using Newtonsoft.Json;

namespace Fluxer.Net;

public class UserCustomStatusJson
{
    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }

    [JsonProperty("emoji_animated")]
    public bool EmojiAnimated { get; set; }

    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }
}
