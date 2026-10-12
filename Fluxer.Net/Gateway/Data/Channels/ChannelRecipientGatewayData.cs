using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class ChannelRecipientGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; } = null!;
}