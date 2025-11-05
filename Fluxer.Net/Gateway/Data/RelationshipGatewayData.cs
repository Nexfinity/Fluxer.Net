using Fluxer.Net.Data.Models;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for RELATIONSHIP_ADD, RELATIONSHIP_UPDATE, and RELATIONSHIP_REMOVE events.
/// </summary>
public class RelationshipGatewayData : IGatewayData
{
	[JsonProperty("relationship")]
	public Relationship Relationship { get; set; } = null!;
}
