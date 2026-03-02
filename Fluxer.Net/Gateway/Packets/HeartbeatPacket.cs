using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Packets;

public class HeartbeatPacket
{
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    [JsonProperty("d")]
    public int? Data { get; set; }
}
