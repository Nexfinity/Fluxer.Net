using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for CALL_CREATE  event.
/// </summary>
public class CallCreateGatewayData
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

    [JsonProperty("recipients")]
    public ulong[]? Recipients { get; set; }

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }
}