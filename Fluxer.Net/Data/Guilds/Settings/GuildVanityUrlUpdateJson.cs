using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The request body for setting a guild's vanity URL code.
/// </summary>
public class GuildVanityUrlUpdateJson
{
    /// <summary>
    /// The new vanity URL code.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }
}
