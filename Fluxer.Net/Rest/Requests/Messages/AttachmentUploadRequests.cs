using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// A file to plan a direct upload for with <see cref="FluxerApiClient.PlanAttachmentUploadsAsync"/>.
/// </summary>
public class PlannedAttachmentUpload
{
    /// <summary>
    /// Your identifier for the file, echoed back in the plan.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the file, 1 to 255 characters.
    /// </summary>
    [JsonProperty("filename")]
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// The size of the file in bytes.
    /// </summary>
    [JsonProperty("file_size")]
    public long FileSize { get; set; }

    /// <summary>
    /// The MIME type the file will be uploaded with.
    /// </summary>
    [JsonProperty("content_type")]
    public string ContentType { get; set; } = "application/octet-stream";
}

internal class PlanAttachmentUploadsRequest
{
    [JsonProperty("attachments")]
    public IEnumerable<PlannedAttachmentUpload> Attachments { get; set; } = Array.Empty<PlannedAttachmentUpload>();
}

/// <summary>
/// A file uploaded with <see cref="FluxerApiClient.UploadAttachmentAsync"/>, ready to send in a message.
/// </summary>
public class UploadedAttachment : AttachmentJson
{
    /// <summary>
    /// The upload key Fluxer gave the file.
    /// </summary>
    [JsonProperty("upload_filename")]
    public string UploadFilename { get; set; } = string.Empty;

    /// <summary>
    /// The size of the uploaded file in bytes.
    /// </summary>
    [JsonProperty("file_size")]
    public long FileSize { get; set; }
}
