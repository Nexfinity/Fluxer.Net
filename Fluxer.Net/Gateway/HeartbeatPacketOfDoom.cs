using Newtonsoft.Json;
using Fluxer.Net.Extensions;

namespace Fluxer.Net.Gateway;

public class HeartbeatPacketOfDoom
{
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    [JsonProperty("d")]
    public int? Data { get; set; }
}
