using Newtonsoft.Json;
using Squll.Net.Extensions;

namespace Squll.Net.Gateway;

public class HeartbeatPacketOfDoom
{
    [JsonProperty("op")]
    public SqullOpCode OpCode { get; set; }

    [JsonProperty("d")]
    public int? Data { get; set; }
}
