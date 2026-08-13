using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class VoiceRingRequest
{
    [JsonProperty("recipients")]
    public ulong[] Recipients { get; set; }
}
