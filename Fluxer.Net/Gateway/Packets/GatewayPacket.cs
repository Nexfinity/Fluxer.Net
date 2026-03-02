using Newtonsoft.Json;
using Fluxer.Net.Extensions;

namespace Fluxer.Net.Gateway.Packets;

public class GatewayPacket
{
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    [JsonConverter(typeof(JsonDerivedTypeConverter<IGatewayData>))]
    [JsonProperty("d")]
    public IGatewayData? Data { get; set; }

    [JsonProperty("s")]
    public int? Sequence { get; set; }

    [JsonProperty("t")]
    public string? Dispatch { get; set; }
}
