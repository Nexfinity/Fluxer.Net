using Newtonsoft.Json;

namespace Fluxer.Net;

public class RefreshedAttachmentUrlJson
{
    [JsonProperty("original")]
    public string Original { get; set; }

    /// <summary>
    /// The URL with a new signature, or the original when it is not an attachment URL.
    /// </summary>
    [JsonProperty("refreshed")]
    public string Refreshed { get; set; }
}

public class RefreshedAttachmentUrlListJson
{
    [JsonProperty("refreshed_urls")]
    public RefreshedAttachmentUrlJson[] Urls { get; set; } = Array.Empty<RefreshedAttachmentUrlJson>();
}
