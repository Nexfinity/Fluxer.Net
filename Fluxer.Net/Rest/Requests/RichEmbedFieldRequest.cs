using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L62"/>
/// </remarks>
public class RichEmbedFieldRequest
{
    /// <summary>
    /// Name of the field
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Value of the field (1-1024 characters)
    /// </summary>
    [MinLength(1)]
    [MaxLength(1024)]
    [JsonProperty("value")]
    public string Value { get; set; }

    /// <summary>
    /// Whether the field should display inline
    /// </summary>
    [JsonProperty("inline")]
    public bool Inline { get; set; } = false;
}
