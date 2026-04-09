using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildEmojiJson : EmojiJson, IGuildEmoji
{
    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson? Creator { get; set; }

    IUser? IGuildEmoji.Creator => Creator;
}
