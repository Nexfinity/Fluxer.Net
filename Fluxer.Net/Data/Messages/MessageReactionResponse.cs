using Newtonsoft.Json;

namespace Fluxer.Net;

public class MessageReactionResponse : Entity
{
    [JsonRequired]
    [JsonProperty("emoji")]
    public ReactionEmojiResponse Emoji { get; set; }

    [JsonRequired]
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("me")]
    public bool? Me { get; set; }
}
