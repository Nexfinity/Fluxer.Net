using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Changes an application. Only the fields that are set change.
/// </summary>
public class UpdateApplicationRequest
{
    /// <summary>
    /// The application name, up to 100 characters.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The redirect URIs for OAuth2.
    /// </summary>
    [JsonProperty("redirect_uris")]
    public IEnumerable<string>? RedirectUrls { get; set; }

    /// <summary>
    /// Whether anyone can invite the bot.
    /// </summary>
    [JsonProperty("bot_public")]
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Whether the bot requires the OAuth2 code grant.
    /// </summary>
    [JsonProperty("bot_require_code_grant")]
    public bool? RequiresCodeGrant { get; set; }
}
