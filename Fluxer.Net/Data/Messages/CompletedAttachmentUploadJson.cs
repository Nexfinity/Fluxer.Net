using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The completed attachment uploads of a request.
/// </summary>
public class CompletedAttachmentUploadListJson
{
    /// <summary>
    /// The completed uploads.
    /// </summary>
    [JsonProperty("uploads")]
    public CompletedAttachmentUploadJson[] Uploads { get; set; } = Array.Empty<CompletedAttachmentUploadJson>();
}

/// <summary>
/// An attachment upload that has been completed.
/// </summary>
public class CompletedAttachmentUploadJson
{
    /// <summary>
    /// The upload key of the completed upload.
    /// </summary>
    [JsonProperty("upload_filename")]
    public string UploadFilename { get; set; }
}
