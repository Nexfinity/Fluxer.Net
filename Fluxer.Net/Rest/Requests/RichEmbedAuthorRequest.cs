using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net.Rest.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L40"/>
/// </remarks>
public class RichEmbedAuthorRequest
{
    /// <summary>
    /// Name of the embed author
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// URL to link from the author name
    /// </summary>
    [MinLength(ApiLimits.UrlTypeMinLength)]
    [MaxLength(ApiLimits.UrlTypeMaxLength)]
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <summary>
    /// URL of the author icon
    /// </summary>
    [MinLength(ApiLimits.UrlTypeMinLength)]
    [MaxLength(ApiLimits.UrlTypeMaxLength)]
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }
}
