using Newtonsoft.Json;

namespace Fluxer.Net;

public class UpdateVoiceRegionRequest
{
    [JsonProperty("region")]
    public string? Region { get; set; }
}
