using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildEmojiJson : EmojiJson
{
    /// <inheritdoc />
    [JsonProperty("user")]
    public User? Creator { get; set; }
}
