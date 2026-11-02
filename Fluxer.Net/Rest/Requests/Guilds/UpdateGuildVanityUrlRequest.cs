using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for changing a guild's vanity URL code.
/// </summary>
public class UpdateGuildVanityUrlRequest
{
    /// <summary>
    /// The new vanity Url code (2-32 characters, alphanumeric and hyphens)
    /// </summary>
    [JsonProperty("code")]
    public string? Code { get; set; }
}
