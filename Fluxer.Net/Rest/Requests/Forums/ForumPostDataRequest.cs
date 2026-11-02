using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for fetching the author and first message of forum posts.
/// </summary>
public class ForumPostDataRequest
{
    /// <summary>
    /// The IDs of the posts to fetch.
    /// </summary>
    [JsonProperty("thread_ids")]
    public IEnumerable<ulong> ThreadIds { get; set; } = Array.Empty<ulong>();
}
