using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Grants an application the consent it asked for.
/// </summary>
public class AuthorizeConsentRequest
{
    /// <summary>
    /// The application client id.
    /// </summary>
    [JsonProperty("client_id")]
    public ulong ClientId { get; set; }

    /// <summary>
    /// The space-separated scopes being granted.
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; }

    /// <summary>
    /// The OAuth2 response type, "code".
    /// </summary>
    [JsonProperty("response_type")]
    public string? ResponseType { get; set; }

    /// <summary>
    /// The URI to send the user to afterwards.
    /// </summary>
    [JsonProperty("redirect_uri")]
    public string? RedirectUrl { get; set; }

    /// <summary>
    /// A random string for CSRF protection.
    /// </summary>
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <summary>
    /// The guild to add the bot to.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <summary>
    /// The group DM to add the bot to.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// The bot permissions being requested.
    /// </summary>
    [JsonProperty("permissions")]
    public string? Permissions { get; set; }

    /// <summary>
    /// The PKCE code challenge.
    /// </summary>
    [JsonProperty("code_challenge")]
    public string? CodeChallenge { get; set; }

    /// <summary>
    /// The PKCE code challenge method, "S256" or "plain".
    /// </summary>
    [JsonProperty("code_challenge_method")]
    public string? CodeChallengeMethod { get; set; }
}
