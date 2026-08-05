using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for RELATIONSHIP_ADD, RELATIONSHIP_UPDATE, and RELATIONSHIP_REMOVE events.
/// </summary>
public class RelationshipGatewayData : RelationshipJson
{
    [JsonProperty("user")]
    public UserJson User { get; set; }
}
public class RelationshipRemoveGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
}