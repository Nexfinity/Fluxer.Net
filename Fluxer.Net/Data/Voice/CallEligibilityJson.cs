using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class CallEligibilityJson : ICallEligibility
{

    /// <inheritdoc />
    [JsonProperty("ringable")]
    public bool IsRingable { get; set; }


    /// <inheritdoc />
    [JsonProperty("silent")]
    public bool IsSilent { get; set; }
}
