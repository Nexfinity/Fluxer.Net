using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net.Gateway.Packets;

public class GatewayPacket
{
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    [JsonProperty("d")]
    public JToken? Data { get; set; }

    [JsonProperty("s")]
    public int? Sequence { get; set; }

    [JsonProperty("t")]
    public string? Dispatch { get; set; }
}
