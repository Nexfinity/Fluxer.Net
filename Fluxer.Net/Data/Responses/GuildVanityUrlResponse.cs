using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class GuildVanityUrlResponse
{
    [JsonProperty("code")]
    public string? Code { get; set; }
    
    [JsonProperty("uses")]
    public int Uses { get; set; }
}
