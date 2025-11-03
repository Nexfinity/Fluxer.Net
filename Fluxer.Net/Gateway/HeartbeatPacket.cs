using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class HeartbeatPacket
{
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    [JsonProperty("d")]
    public int? Data { get; set; }
}
