using Newtonsoft.Json;

namespace Fluxer.Net;

public class ThreadListJson
{
    [JsonProperty("threads")]
    public ChannelJson[] Threads { get; set; } = Array.Empty<ChannelJson>();

    [JsonProperty("members")]
    public ThreadMemberJson[] Members { get; set; } = Array.Empty<ThreadMemberJson>();

    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("total_results")]
    public int? TotalResults { get; set; }
}
