using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class EmojiChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the emoji.
    /// </summary>
    [JsonProperty("emoji_id")]
    public ulong EmojiId { get; set; }

    /// <summary>
    /// The name of the emoji.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    [JsonProperty("animated")]
    public bool IsAnimated { get; set; }

    /// <summary>
    /// The id of the user that uploaded the emoji.
    /// </summary>
    [JsonProperty("creator_id")]
    public ulong CreatorId { get; set; }
}
