using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class RelationshipGatewayData : RelationshipJson
{
    [JsonProperty("user")]
    public UserJson User { get; set; }
}

internal class RelationshipRemoveGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }
}