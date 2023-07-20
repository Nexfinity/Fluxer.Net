using Newtonsoft.Json;
using Squll.Net.Extensions;

namespace Squll.Net.Gateway;

public class UntypedDataGatewayPacket
{
    [JsonProperty("op")]
    public SqullOpCode OpCode { get; set; }

    [JsonProperty("d")]
    public object? Data { get; set; }

    [JsonProperty("s")]
    public int? Sequence { get; set; }

    [JsonProperty("t")]
    public string Dispatch { get; set; }
}
