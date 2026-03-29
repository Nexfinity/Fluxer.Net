using Newtonsoft.Json;

namespace Fluxer.Net;

public class VoiceRingRequest
{
    [JsonProperty("recipients")]
    public ulong[] Recipients { get; set; }
}
