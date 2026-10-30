using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// A tag in a forum's tag list. Include <see cref="Id"/> to keep an existing tag and leave it out to create one.
/// </summary>
public class ForumTagUpdateRequest
{
    /// <summary>
    /// The ID of an existing tag to keep.
    /// </summary>
    [JsonProperty("id")]
    public Optional<ulong> Id { get; set; }

    /// <summary>
    /// The name of the tag, 1 to 50 characters.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether only moderators can add or remove the tag.
    /// </summary>
    [JsonProperty("moderated")]
    public Optional<bool> IsModerated { get; set; }

    /// <summary>
    /// The ID of a custom guild emoji for the tag.
    /// </summary>
    [JsonProperty("emoji_id")]
    public Optional<ulong?> EmojiId { get; set; }

    /// <summary>
    /// The unicode emoji for the tag.
    /// </summary>
    [JsonProperty("emoji_name")]
    public Optional<string?> EmojiName { get; set; }
}
