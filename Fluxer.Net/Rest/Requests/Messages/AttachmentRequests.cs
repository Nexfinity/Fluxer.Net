using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class RefreshAttachmentUrlsRequest
{
    [JsonProperty("attachment_urls")]
    public IEnumerable<string> Urls { get; set; } = Array.Empty<string>();
}

public class CompleteAttachmentUploadsRequest
{
    [JsonProperty("uploads")]
    public IEnumerable<CompleteAttachmentUploadItem> Uploads { get; set; } = Array.Empty<CompleteAttachmentUploadItem>();
}

public class CompleteAttachmentUploadItem
{
    /// <summary>
    /// The upload_filename Fluxer returned when the upload was planned.
    /// </summary>
    [JsonProperty("upload_filename")]
    public string UploadFilename { get; set; }

    /// <summary>
    /// The upload_id Fluxer returned when the upload was planned.
    /// </summary>
    [JsonProperty("upload_id")]
    public string UploadId { get; set; }
}
