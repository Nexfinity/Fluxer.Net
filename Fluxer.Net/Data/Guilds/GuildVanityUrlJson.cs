using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildVanityUrlJson
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uses")]
    public int Uses { get; set; }
}
