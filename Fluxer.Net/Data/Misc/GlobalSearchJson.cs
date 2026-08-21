using Newtonsoft.Json;

namespace Fluxer.Net;

public class GlobalSearchJson
{
    [JsonProperty("messages")]
    public MessageJson[] Messages { get; set; }

    [JsonProperty("channels")]
    public ChannelJson[] Channels { get; set; }

    [JsonProperty("total")]
    public ulong Total { get; set; }

    [JsonProperty("hits_per_page")]
    public int HitsPerPage { get; set; }

    [JsonProperty("page")]
    public int Page { get; set; }
}
