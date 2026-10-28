using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class PreloadMessagesRequest
{
    [JsonProperty("channels")]
    public IEnumerable<ulong> ChannelIds { get; set; } = Array.Empty<ulong>();
}
