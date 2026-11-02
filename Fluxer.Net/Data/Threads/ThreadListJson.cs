using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The response of a thread list request.
/// </summary>
public class ThreadListJson
{
    /// <summary>
    /// The threads in the list.
    /// </summary>
    [JsonProperty("threads")]
    public ChannelJson[] Threads { get; set; } = Array.Empty<ChannelJson>();

    /// <summary>
    /// The current user's thread member for each listed thread it joined.
    /// </summary>
    [JsonProperty("members")]
    public ThreadMemberJson[] Members { get; set; } = Array.Empty<ThreadMemberJson>();

    /// <summary>
    /// Whether more threads can be fetched.
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    /// <summary>
    /// The total number of threads matching a search, or null when the list is not a search result.
    /// </summary>
    [JsonProperty("total_results")]
    public int? TotalResults { get; set; }
}
