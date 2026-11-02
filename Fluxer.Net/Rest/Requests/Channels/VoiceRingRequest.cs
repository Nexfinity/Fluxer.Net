using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for ringing the users of a call.
/// </summary>
public class VoiceRingRequest
{
    /// <summary>
    /// The IDs of the users to ring.
    /// </summary>
    [JsonProperty("recipients")]
    public ulong[] Recipients { get; set; }
}
