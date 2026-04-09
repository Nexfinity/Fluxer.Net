using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class MessageReactionJson : IMessageReaction
{
    /// <inheritdoc />
    [JsonProperty("emoji")]
    public EmojiJson Emoji { get; set; }

    /// <inheritdoc />
    [JsonProperty("count")]
    public int Count { get; set; }

    /// <inheritdoc />
    [JsonProperty("me")]
    public bool? Me { get; set; }

    IEmoji IMessageReaction.Emoji => Emoji;
}
