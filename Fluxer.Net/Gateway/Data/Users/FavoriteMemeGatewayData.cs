using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for FAVORITE_MEME_CREATE, FAVORITE_MEME_UPDATE, and FAVORITE_MEME_DELETE events.
/// </summary>
public class FavoriteMemeGatewayData : FavoriteGifJson
{

}

public class FavoriteMemeDeleteGatewayData : FavoriteGifJson
{
    [JsonProperty("meme_id")]
    public new string Id { get; set; }
}