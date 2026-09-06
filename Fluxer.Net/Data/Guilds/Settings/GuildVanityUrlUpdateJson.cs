using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildVanityUrlUpdateJson
{
    /// <summary>
    /// The new vanity Url code
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }
}
