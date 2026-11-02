using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The emoji a forum shows as the default reaction on its posts.
/// </summary>
public class ForumReactionEmoji
{
    /// <summary>
    /// The ID of the custom emoji.
    /// </summary>
    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    /// <summary>
    /// The name of the emoji.
    /// </summary>
    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }
}
