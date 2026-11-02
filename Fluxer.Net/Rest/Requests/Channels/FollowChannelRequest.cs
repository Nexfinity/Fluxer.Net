using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for following an announcement channel into another channel.
/// </summary>
public class FollowChannelRequest
{
    /// <summary>
    /// The ID of the channel that receives the followed announcement channel's messages.
    /// </summary>
    [JsonProperty("webhook_channel_id")]
    public ulong ChannelId { get; set; }
}
