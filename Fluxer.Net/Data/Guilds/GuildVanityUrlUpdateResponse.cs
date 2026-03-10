using Newtonsoft.Json;

namespace Fluxer.Net.Data.Guilds;

public class GuildVanityUrlUpdateResponse : Entity
{
    /// <summary>
    /// The new vanity Url code
    /// </summary>
    [JsonRequired]
    [JsonProperty("code")]
    public string Code { get; set; }
}
