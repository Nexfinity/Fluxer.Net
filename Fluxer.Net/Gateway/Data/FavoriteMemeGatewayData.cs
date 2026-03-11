using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for FAVORITE_MEME_CREATE, FAVORITE_MEME_UPDATE, and FAVORITE_MEME_DELETE events.
/// </summary>
public class FavoriteMemeGatewayData : IGatewayData
{
	[JsonProperty("favorite_meme")]
	public FavoriteMeme FavoriteMeme { get; set; } = null!;
}
