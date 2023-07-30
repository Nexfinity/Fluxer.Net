using Newtonsoft.Json;

namespace Squll.Net.Gateway.Data;

public class EntityRemovedGatewayData : IGatewayData
{
    [JsonProperty("id")]
    public ulong? Id { get; set; }
    [JsonProperty("squad_id")]
    public ulong? SquadId { get; set; }
    [JsonProperty("space_id")]
    public ulong? SpaceId { get; set; }
}
