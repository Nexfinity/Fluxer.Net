using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for USER_NOTE_UPDATE event when a user note is updated.
/// </summary>
public class UserNoteUpdateGatewayData : IGatewayData
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("note")]
	public string? Note { get; set; }
}
