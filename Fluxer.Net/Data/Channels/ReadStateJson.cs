using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The read state of a channel for one user.
/// </summary>
public class ReadStateJson
{
    /// <summary>
    /// The ID of the user the read state belongs to.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The ID of the channel.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The ID of the last message the user has read.
    /// </summary>
    [JsonProperty("message_id")]
    public ulong? LastMessageId { get; set; }

    /// <summary>
    /// The number of unread mentions in the channel.
    /// </summary>
    [JsonProperty("mention_count")]
    public int MentionCount { get; set; }

    /// <summary>
    /// The time of the last pin the user has seen.
    /// </summary>
    [JsonProperty("last_pin_timestamp")]
    public DateTimeOffset? LastPinAt { get; set; }
}
