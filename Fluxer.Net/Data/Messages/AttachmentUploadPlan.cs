using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Where and how to upload one planned file.
/// </summary>
public class AttachmentUploadPlan
{
    /// <summary>
    /// The identifier given when planning.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// The original name of the file.
    /// </summary>
    [JsonProperty("filename")]
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// The expected size of the file in bytes.
    /// </summary>
    [JsonProperty("file_size")]
    public long FileSize { get; set; }

    /// <summary>
    /// The MIME type the file must be uploaded with.
    /// </summary>
    [JsonProperty("content_type")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The upload key to reference when sending the message.
    /// </summary>
    [JsonProperty("upload_filename")]
    public string UploadFilename { get; set; } = string.Empty;

    /// <summary>
    /// <c>singlepart</c> for one PUT to <see cref="UploadUrl"/>, or <c>multipart</c> for one PUT per entry in <see cref="Parts"/>.
    /// </summary>
    [JsonProperty("upload_mode")]
    public string UploadMode { get; set; } = string.Empty;

    /// <summary>
    /// Whether the file is uploaded in parts.
    /// </summary>
    [JsonIgnore]
    public bool IsMultipart => UploadMode == "multipart";

    /// <summary>
    /// The URL to PUT the whole file to, for a single-part upload.
    /// </summary>
    [JsonProperty("upload_url")]
    public string? UploadUrl { get; set; }

    /// <summary>
    /// The multipart upload identifier, needed to finish the upload.
    /// </summary>
    [JsonProperty("upload_id")]
    public string? UploadId { get; set; }

    /// <summary>
    /// The size in bytes of every part except the last.
    /// </summary>
    [JsonProperty("part_size")]
    public long? PartSize { get; set; }

    /// <summary>
    /// The URL for each part, in order.
    /// </summary>
    [JsonProperty("parts")]
    public AttachmentUploadPart[]? Parts { get; set; }
}

/// <summary>
/// One part of a multipart upload.
/// </summary>
public class AttachmentUploadPart
{
    /// <summary>
    /// The part number, starting at 1.
    /// </summary>
    [JsonProperty("part_number")]
    public int PartNumber { get; set; }

    /// <summary>
    /// The URL to PUT this part to.
    /// </summary>
    [JsonProperty("upload_url")]
    public string UploadUrl { get; set; } = string.Empty;
}

internal class AttachmentUploadPlanListJson
{
    [JsonProperty("attachments")]
    public AttachmentUploadPlan[] Attachments { get; set; } = Array.Empty<AttachmentUploadPlan>();
}
