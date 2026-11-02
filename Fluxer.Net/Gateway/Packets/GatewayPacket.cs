using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A message sent to or received from the gateway.
/// </summary>
public class GatewayPacket
{
    /// <summary>
    /// The operation code of the packet.
    /// </summary>
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    /// <summary>
    /// The payload of the packet, as raw JSON.
    /// </summary>
    [JsonProperty("d")]
    public JToken? Data { get; set; }

    /// <summary>
    /// The sequence number of a dispatch, or null for other packets.
    /// </summary>
    [JsonProperty("s")]
    public int? Sequence { get; set; }

    /// <summary>
    /// The event name of a dispatch, such as MESSAGE_CREATE, or null for other packets.
    /// </summary>
    [JsonProperty("t")]
    public string? Dispatch { get; set; }
}
