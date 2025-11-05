using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway user data matching the UserPartialResponse API model
/// </summary>
public class UserGatewayData : IGatewayData
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; } = null!;

	[JsonProperty("discriminator")]
	public string Discriminator { get; set; } = null!;

	[JsonProperty("avatar")]
	public string? Avatar { get; set; }

	[JsonProperty("bot")]
	public bool IsBot { get; set; }

	[JsonProperty("system")]
	public bool IsSystem { get; set; }

	[JsonProperty("flags")]
	public int Flags { get; set; }

	[JsonProperty("banner")]
	public string? Banner { get; set; }

	[JsonProperty("accent_color")]
	public int? AccentColor { get; set; }
}
