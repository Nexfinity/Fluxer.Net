using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

public class GuildVanityUrlResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("uses")]
    public int Uses { get; set; }
}
