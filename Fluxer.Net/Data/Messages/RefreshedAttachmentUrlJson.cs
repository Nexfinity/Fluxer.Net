using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// An attachment URL and its refreshed form.
/// </summary>
public class RefreshedAttachmentUrlJson
{
    /// <summary>
    /// The URL that was sent for refreshing.
    /// </summary>
    [JsonProperty("original")]
    public string Original { get; set; }

    /// <summary>
    /// The URL with a new signature, or the original when it is not an attachment URL.
    /// </summary>
    [JsonProperty("refreshed")]
    public string Refreshed { get; set; }
}

/// <summary>
/// The result of refreshing attachment URLs.
/// </summary>
public class RefreshedAttachmentUrlListJson
{
    /// <summary>
    /// The refreshed URLs.
    /// </summary>
    [JsonProperty("refreshed_urls")]
    public RefreshedAttachmentUrlJson[] Urls { get; set; } = Array.Empty<RefreshedAttachmentUrlJson>();
}
