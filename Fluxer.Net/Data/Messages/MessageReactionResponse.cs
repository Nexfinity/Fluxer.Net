using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageReactionResponse
{
    [JsonRequired]
    [JsonProperty("emoji")]
    public ReactionEmojiJson Emoji { get; set; }

    [JsonRequired]
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("me")]
    public bool? Me { get; set; }
}
