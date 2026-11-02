using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The body of a request to acknowledge a message.
/// </summary>
public class MessageAckJson
{
    /// <summary>
    /// Whether the acknowledgement was made manually.
    /// </summary>
    [JsonProperty("manual")]
    public bool Manual { get; set; }

    /// <summary>
    /// The mention count sent with the acknowledgement.
    /// </summary>
    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }
}
