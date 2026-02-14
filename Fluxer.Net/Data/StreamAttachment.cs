using Fluxer.Net.Data.Models;
using Newtonsoft.Json;

namespace Fluxer.Net.Data;

public class StreamAttachment : Attachment
{
    [JsonIgnore]
    public required Stream Stream { get; init; }
}
