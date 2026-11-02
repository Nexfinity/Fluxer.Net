using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A custom status that a user has set, as the API sends it.
/// </summary>
public class UserCustomStatusJson
{
    /// <summary>
    /// The text of the status, or null when it has none.
    /// </summary>
    [JsonProperty("text")]
    public string? Text { get; set; }

    /// <summary>
    /// The id of the custom emoji of the status, or null when it has none.
    /// </summary>
    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    /// <summary>
    /// The name of the emoji of the status, or null when it has none.
    /// </summary>
    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }

    /// <summary>
    /// Whether the emoji of the status is animated.
    /// </summary>
    [JsonProperty("emoji_animated")]
    public bool EmojiAnimated { get; set; }

    /// <summary>
    /// When the status expires, or null when it does not expire.
    /// </summary>
    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }
}
