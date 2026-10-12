using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildDeleteGatewayData
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("unavailable")]
    public bool? IsUnavailable { get; set; }
}