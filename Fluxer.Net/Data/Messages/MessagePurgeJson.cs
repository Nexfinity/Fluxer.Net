using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The result of a bulk message delete.
/// </summary>
public class MessagePurgeJson
{
    /// <summary>
    /// The number of messages deleted.
    /// </summary>
    [JsonProperty("deleted_count")]
    public int DeletedCount { get; set; }
}
