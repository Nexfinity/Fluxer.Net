using Newtonsoft.Json;

namespace Fluxer.Net;

public class ForumReactionEmoji
{
    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }
}
