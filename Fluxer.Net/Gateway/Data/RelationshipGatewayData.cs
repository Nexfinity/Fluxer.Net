using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for RELATIONSHIP_ADD, RELATIONSHIP_UPDATE, and RELATIONSHIP_REMOVE events.
/// </summary>
public class RelationshipGatewayData : IGatewayData
{
	[JsonProperty("relationship")]
	public RelationshipJson Relationship { get; set; } = null!;
}
