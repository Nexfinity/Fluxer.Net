using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Fetches message windows from several channels in one request.
/// </summary>
public class BulkFetchMessagesRequest
{
    /// <summary>
    /// The window to fetch from each channel, between 1 and 25.
    /// </summary>
    [JsonProperty("requests")]
    public IEnumerable<BulkFetchMessagesChannelRequest> Requests { get; set; }
}

/// <summary>
/// The message window to fetch from one channel.
/// </summary>
public class BulkFetchMessagesChannelRequest
{
    /// <summary>
    /// The channel to fetch from.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// How many messages to return, between 1 and 50.
    /// </summary>
    [JsonProperty("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// Get messages before this message.
    /// </summary>
    [JsonProperty("before")]
    public ulong? Before { get; set; }

    /// <summary>
    /// Get messages after this message.
    /// </summary>
    [JsonProperty("after")]
    public ulong? After { get; set; }

    /// <summary>
    /// Get messages around this message.
    /// </summary>
    [JsonProperty("around")]
    public ulong? Around { get; set; }
}
