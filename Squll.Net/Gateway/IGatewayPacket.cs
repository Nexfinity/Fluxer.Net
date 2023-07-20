using Newtonsoft.Json;

namespace Squll.Net.Gateway;

public interface IGatewayPacket
{
    [JsonProperty("op")]
    public SqullOpCode OpCode { get; }

    [JsonProperty("d")]
    public IGatewayData? Data { get; }

    [JsonProperty("s")]
    public abstract int? Sequence { get; }
}
