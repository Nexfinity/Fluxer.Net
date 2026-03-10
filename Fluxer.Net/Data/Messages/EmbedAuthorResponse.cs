using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/EmbedSchemas.tsx#L24"/>
/// </remarks>
public class EmbedAuthorResponse
{
    /// <summary>
    /// The name of the author
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The URL of the author
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; set; }

    /// <summary>
    /// The URL of the author icon
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }

    /// <summary>
    /// The proxied URL of the author icon
    /// </summary>
    [JsonProperty("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}
