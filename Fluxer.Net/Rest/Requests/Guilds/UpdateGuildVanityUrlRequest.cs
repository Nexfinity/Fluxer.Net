using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class UpdateGuildVanityUrlRequest
{
    /// <summary>
    /// The new vanity Url code (2-32 characters, alphanumeric and hyphens)
    /// </summary>
    [JsonProperty("code")]
    public string? Code { get; set; }
}
