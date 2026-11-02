using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a post in a forum channel.
/// </summary>
public class CreatePostRequest
{
    /// <summary>
    /// The title of the post.
    /// </summary>
    [JsonProperty("name")]
    public string Title { get; set; }

    /// <summary>
    /// How long the post can be inactive before it is archived.
    /// </summary>
    [JsonProperty("auto_archive_duration")]
    public ThreadArchiveDuration? ArchiveDuration { get; set; }

    /// <summary>
    /// The slowmode delay in seconds between messages from one user.
    /// </summary>
    [JsonProperty("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    /// <summary>
    /// The first message of the post.
    /// </summary>
    [JsonProperty("message")]
    public MessageRequest Message { get; set; }

    /// <summary>
    /// The IDs of the forum tags applied to the post.
    /// </summary>
    [JsonProperty("applied_tags")]
    public ulong[]? Tags { get; set; }
}
