using Newtonsoft.Json;

namespace Fluxer.Net;

public class StreamAttachment : Attachment
{
    [JsonIgnore]
    public required Stream Stream { get; init; }
}
