using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class FollowChannelRequest
{
    [JsonProperty("webhook_channel_id")]
    public ulong ChannelId { get; set; }
}
