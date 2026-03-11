using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/AttachmentSchemas.tsx#L54"/>
/// </remarks>
public class ClientAttachmentReferenceRequest
{
    /// <summary>
    /// The identifier of the attachment being referenced (snowflake ID or file index)
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// A new filename for the attachment
    /// </summary>
    [MinLength(ApiLimits.FilenameTypeMinLength)]
    [MaxLength(ApiLimits.FilenameTypeMaxLength)]
    [JsonProperty("filename")]
    public string? Filename { get; set; }
}
