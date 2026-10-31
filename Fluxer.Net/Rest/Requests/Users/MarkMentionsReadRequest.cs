using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Removes messages from your recent mentions.
/// </summary>
public class MarkMentionsReadRequest
{
    /// <summary>
    /// The messages to remove, between 1 and 100.
    /// </summary>
    [JsonProperty("message_ids")]
    public IEnumerable<ulong> MessageIds { get; set; }
}
