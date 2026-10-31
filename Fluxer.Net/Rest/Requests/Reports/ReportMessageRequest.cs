using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Reports a message.
/// </summary>
public class ReportMessageRequest
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

    /// <summary>
    /// One of harassment, hate_speech, violent_content, spam, nsfw_violation, illegal_activity, doxxing, self_harm, child_safety, malicious_links, impersonation or other.
    /// </summary>
    [JsonProperty("category")]
    public string Category { get; set; }
}
