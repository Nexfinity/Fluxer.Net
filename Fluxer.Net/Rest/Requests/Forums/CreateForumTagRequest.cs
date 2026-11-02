using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a tag on a forum channel.
/// </summary>
public class CreateForumTagRequest
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Whether the tag is moderated.
    /// </summary>
    [JsonProperty("moderated")]
    public bool? IsModerated { get; set; }

    /// <summary>
    /// The name of the emoji shown with the tag.
    /// </summary>
    [JsonProperty("emoji_name")]
    public string? EmojiName { get; set; }

    /// <summary>
    /// The ID of the custom emoji shown with the tag.
    /// </summary>
    [JsonProperty("emoji_id")]
    public ulong? EmojiId { get; set; }
}
