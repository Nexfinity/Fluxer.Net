using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/EmbedSchemas.tsx#L61"/>
/// </remarks>
public class EmbedFieldResponse
{
    /// <summary>
    /// The name of the field
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The value of the field
    /// </summary>
    [JsonRequired]
    [JsonProperty("value")]
    public string Value { get; set; }

    /// <summary>
    /// Whether the field should be displayed inline
    /// </summary>
    [JsonProperty("inline")]
    public bool Inline { get; set; }
}
