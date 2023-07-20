using Newtonsoft.Json;
using Squll.Net.Extensions;

namespace Squll.Net.Gateway;

public class GatewayPacket
{
    [JsonProperty("op")]
    public SqullOpCode OpCode { get; set; }

    [JsonConverter(typeof(JsonDerivedTypeConverter<IGatewayData>))]
    [JsonProperty("d")]
    public IGatewayData? Data { get; set; }

    [JsonProperty("s")]
    public int? Sequence { get; set; }

    [JsonProperty("t")]
    public string Dispatch { get; set; }
}
