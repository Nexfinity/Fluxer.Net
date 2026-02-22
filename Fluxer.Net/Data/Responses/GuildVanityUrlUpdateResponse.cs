using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class GuildVanityUrlUpdateResponse
{
    /// <summary>
    /// The new vanity Url code
    /// </summary>
    [JsonRequired]
    [JsonProperty("code")]
    public string Code { get; set; }
}
