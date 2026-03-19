using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for SAVED_MESSAGE_CREATE and SAVED_MESSAGE_DELETE events.
/// </summary>
public class SavedMessageGatewayData : IGatewayData
{
	[JsonProperty("saved_message")]
	public SavedMessageJson SavedMessage { get; set; } = null!;
}
