using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for CALL_CREATE  event.
/// </summary>
public class CallDeleteGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("unavailable")]
    public bool? Unavailable { get; set; }
}
