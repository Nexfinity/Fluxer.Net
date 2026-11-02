using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for refreshing the URLs of expired attachments.
/// </summary>
public class RefreshAttachmentUrlsRequest
{
    /// <summary>
    /// The attachment URLs to refresh.
    /// </summary>
    [JsonProperty("attachment_urls")]
    public IEnumerable<string> Urls { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Request body for completing one or more attachment uploads.
/// </summary>
public class CompleteAttachmentUploadsRequest
{
    /// <summary>
    /// The uploads to complete.
    /// </summary>
    [JsonProperty("uploads")]
    public IEnumerable<CompleteAttachmentUploadItem> Uploads { get; set; } = Array.Empty<CompleteAttachmentUploadItem>();
}

/// <summary>
/// One attachment upload to complete.
/// </summary>
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
