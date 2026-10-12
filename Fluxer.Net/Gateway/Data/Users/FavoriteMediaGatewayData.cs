using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class FavoriteMediaGatewayData : FavoriteMediaJson
{

}

internal class FavoriteMediaDeleteGatewayData
{
    [JsonProperty("meme_id")]
    public string Id { get; set; }
}