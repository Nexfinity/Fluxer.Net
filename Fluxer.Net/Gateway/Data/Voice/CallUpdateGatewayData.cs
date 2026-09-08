using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for CALL_UPDATE  event.
/// </summary>
public class CallUpdateGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("region")]
    public string? Region { get; set; }

    [JsonProperty("ringing")]
    public ulong[] RingingUsers { get; set; }

    [JsonProperty("voice_states")]
    public VoiceStateJson[] VoiceStates { get; set; }
}