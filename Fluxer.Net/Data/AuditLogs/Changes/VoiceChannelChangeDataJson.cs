using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class VoiceChannelChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the voice channel.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}
