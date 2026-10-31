using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// The answers of a finished message report flow.
/// </summary>
public class ReportFlowMessageSubmissionRequest : ReportFlowSubmissionRequest
{
    /// <summary>
    /// The channel of the message.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The message being reported.
    /// </summary>
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }
}
