using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net.Data.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L55"/>
/// </remarks>
public class RichEmbedFooterRequest
{
    /// <summary>
    /// Footer text (1-2048 characters)
    /// </summary>
    [MinLength(1)]
    [MaxLength(2048)]
    [JsonRequired]
    [JsonProperty("text")]
    public string Text { get; set; }

    /// <summary>
    /// URL of the footer icon
    /// </summary>
    [MinLength(ApiLimits.UrlTypeMinLength)]
    [MaxLength(ApiLimits.UrlTypeMaxLength)]
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }
}
