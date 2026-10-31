using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Messages fetched from several channels in one request.
/// </summary>
public class BulkMessagesJson
{
    /// <summary>
    /// The messages of each requested channel.
    /// </summary>
    [JsonProperty("channels")]
    public BulkMessagesChannelJson[] Channels { get; set; }
}

/// <summary>
/// The messages fetched for one channel.
/// </summary>
public class BulkMessagesChannelJson
{
    /// <summary>
    /// The channel the messages came from.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The messages.
    /// </summary>
    [JsonProperty("messages")]
    public MessageJson[] Messages { get; set; }
}
