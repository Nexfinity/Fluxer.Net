using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for CHANNEL_RECIPIENT_ADD and CHANNEL_RECIPIENT_REMOVE events.
/// </summary>
public class ChannelRecipientGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; } = null!;
}
