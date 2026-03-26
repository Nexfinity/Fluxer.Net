using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageReactionResponse
{
    [JsonProperty("emoji")]
    public EmojiJson Emoji { get; set; }

    [JsonRequired]
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("me")]
    public bool? Me { get; set; }
}
