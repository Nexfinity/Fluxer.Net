using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class EntityRemovedGatewayData : IGatewayData
{
    [JsonProperty("id")]
    public ulong? Id { get; set; }
    [JsonProperty("community_id")]
    public ulong? CommunityId { get; set; }
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }
}
