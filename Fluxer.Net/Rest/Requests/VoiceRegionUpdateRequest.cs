using Newtonsoft.Json;

namespace Fluxer.Net;

public class VoiceRegionUpdateRequest
{
    [JsonProperty("region")]
    public string? Region { get; set; }
}
