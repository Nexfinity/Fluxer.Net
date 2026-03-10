using Newtonsoft.Json;

namespace Fluxer.Net.Data.Voice;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/channel/ChannelSchemas.tsx#L44"/>
/// </remarks>
public class CallEligibilityResponse
{
    /// <summary>
    /// Whether the current user can ring this call
    /// </summary>
    [JsonProperty("ringable")]
    public bool Ringable { get; set; }

    /// <summary>
    /// Whether the call should be joined silently
    /// </summary>
    [JsonProperty("silent")]
    public bool Silent { get; set; }
}
