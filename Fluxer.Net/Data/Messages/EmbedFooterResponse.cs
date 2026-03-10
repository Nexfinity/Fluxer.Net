using Newtonsoft.Json;

namespace Fluxer.Net.Data.Messages;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/EmbedSchemas.tsx#L33"/>
/// </remarks>
public class EmbedFooterResponse
{
    /// <summary>
    /// The footer text
    /// </summary>
    [JsonRequired]
    [JsonProperty("text")]
    public string Text { get; set; }

    /// <summary>
    /// The URL of the footer icon
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; set; }

    /// <summary>
    /// The proxied URL of the footer icon
    /// </summary>
    [JsonProperty("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}
