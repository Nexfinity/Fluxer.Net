using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class ForumTagJson : IForumTag
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }

    /// <inheritdoc />
    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }

    /// <inheritdoc />
    [JsonProperty("moderated")]
    public bool IsModerated { get; set; }
}
