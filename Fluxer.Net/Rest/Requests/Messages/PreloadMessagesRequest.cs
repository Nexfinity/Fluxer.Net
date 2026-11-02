using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for preloading the latest messages of several channels.
/// </summary>
public class PreloadMessagesRequest
{
    /// <summary>
    /// The IDs of the channels to preload messages for.
    /// </summary>
    [JsonProperty("channels")]
    public IEnumerable<ulong> ChannelIds { get; set; } = Array.Empty<ulong>();
}
