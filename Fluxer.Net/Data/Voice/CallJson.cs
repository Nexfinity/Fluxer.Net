using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class CallJson : ICall
{
    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <inheritdoc />
    [JsonProperty("region")]
    public string? Region { get; set; }

    /// <inheritdoc />
    [JsonProperty("ringing")]
    public ulong[] RingingUsers { get; set; }

    /// <inheritdoc />
    [JsonProperty("voice_states")]
    public VoiceStateJson[] VoiceStates { get; set; }

    /// <inheritdoc />
    [JsonProperty("recipients")]
    public ulong[]? Recipients { get; set; }

    /// <inheritdoc />
    [JsonProperty("created_at")]
    [JsonConverter(typeof(UnixDateConverter))]
    public DateTimeOffset? CreatedAt { get; set; }

    IVoiceState[] ICall.VoiceStates => VoiceStates;
}
