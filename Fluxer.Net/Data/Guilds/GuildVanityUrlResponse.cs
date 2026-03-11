using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildVanityUrlResponse : Entity
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uses")]
    public int Uses { get; set; }
}
