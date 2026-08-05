using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for USER_PINNED_DMS_UPDATE event when pinned DMs are updated.
/// </summary>
public class UserPinnedDmsUpdateGatewayData
{
    [JsonProperty("pinned")]
    public List<ulong> PinnedChannelIds { get; set; } = new();
}
