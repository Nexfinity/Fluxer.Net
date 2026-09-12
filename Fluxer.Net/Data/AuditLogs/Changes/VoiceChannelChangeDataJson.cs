using Newtonsoft.Json;

namespace Fluxer.Net;

public class VoiceChannelChangeDataJson : IAuditLogData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }
}
