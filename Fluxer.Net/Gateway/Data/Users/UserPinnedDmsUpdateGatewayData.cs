using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class UserPinnedDMsUpdateGatewayData
{
    [JsonProperty("pinned")]
    public List<ulong> PinnedChannelIds { get; set; } = new();
}