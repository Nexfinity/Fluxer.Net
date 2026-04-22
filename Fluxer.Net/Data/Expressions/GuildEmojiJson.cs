using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildEmojiJson : EmojiJson, IGuildEmoji
{
    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson? Creator { get; set; }

    /// <inheritdoc />
    public string? GetEmojiUrl(int size = 160)
    {
        return $"https://fluxerusercontent.com/emojis/{Id}.webp?size={size}";
    }

    IUser? IGuildEmoji.Creator => Creator;
}
