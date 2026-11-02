using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for changing the voice region of a call.
/// </summary>
public class UpdateVoiceRegionRequest
{
    /// <summary>
    /// The ID of the voice region.
    /// </summary>
    [JsonProperty("region")]
    public string? Region { get; set; }
}
