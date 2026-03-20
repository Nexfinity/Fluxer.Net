using Newtonsoft.Json;

namespace Fluxer.Net;

public class StreamAttachment : AttachmentJson
{
    [JsonIgnore]
    public required Stream Stream { get; init; }
}
