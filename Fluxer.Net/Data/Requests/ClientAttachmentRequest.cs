using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net.Data.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/AttachmentSchemas.tsx#L48"/>
/// </remarks>
public class ClientAttachmentRequest
{
    /// <summary>
    /// The client-side identifier for this attachment
    /// </summary>
    [JsonProperty("id")]
    [JsonRequired]
    public int Id { get; set; }

    /// <summary>
    /// The name of the file being uploaded
    /// </summary>
    [MinLength(ApiLimits.FilenameTypeMinLength)]
    [MaxLength(ApiLimits.FilenameTypeMaxLength)]
    [JsonProperty("filename")]
    public string? Filename { get; set; }
}
