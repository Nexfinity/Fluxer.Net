using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class UpdateVoiceRegionRequest
{
    [JsonProperty("region")]
    public string? Region { get; set; }
}
