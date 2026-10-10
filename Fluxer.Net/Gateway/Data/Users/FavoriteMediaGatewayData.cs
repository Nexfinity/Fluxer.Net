using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for FAVORITE_MEME_CREATE, FAVORITE_MEME_UPDATE, and FAVORITE_MEME_DELETE events.
/// </summary>
public class FavoriteMediaGatewayData : FavoriteMediaJson
{

}

public class FavoriteMediaDeleteGatewayData
{
    /// <summary>
    /// Deleted media ID.
    /// </summary>
    [JsonProperty("meme_id")]
    public string Id { get; set; }
}