using Newtonsoft.Json;

namespace Fluxer.Net;

public class CompletedAttachmentUploadListJson
{
    [JsonProperty("uploads")]
    public CompletedAttachmentUploadJson[] Uploads { get; set; } = Array.Empty<CompletedAttachmentUploadJson>();
}

public class CompletedAttachmentUploadJson
{
    [JsonProperty("upload_filename")]
    public string UploadFilename { get; set; }
}
