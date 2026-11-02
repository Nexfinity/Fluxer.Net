using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// The heartbeat sent to the gateway at the interval from Hello.
/// </summary>
public class HeartbeatPacket
{
    /// <summary>
    /// The operation code, which is Heartbeat.
    /// </summary>
    [JsonProperty("op")]
    public FluxerOpCode OpCode { get; set; }

    /// <summary>
    /// The last sequence number received, or null when none has been received.
    /// </summary>
    [JsonProperty("d")]
    public int? Data { get; set; }
}
