using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for deleting several messages in one channel.
/// </summary>
public class BulkDeleteMessagesRequest
{
    /// <summary>
    /// The IDs of the messages to delete.
    /// </summary>
    [JsonProperty("message_ids")]
    public HashSet<ulong> MessageIds { get; set; }
}
