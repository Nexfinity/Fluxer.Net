using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// An application you authorized through OAuth2.
/// </summary>
public class OAuthAuthorizationJson
{
    /// <summary>
    /// The application that was authorized.
    /// </summary>
    [JsonProperty("application")]
    public OAuthAuthorizationApplicationJson Application { get; set; }

    /// <summary>
    /// The granted scopes.
    /// </summary>
    [JsonProperty("scopes")]
    public string[] Scopes { get; set; }

    /// <summary>
    /// When the authorization was granted.
    /// </summary>
    [JsonProperty("authorized_at")]
    public DateTimeOffset AuthorizedAt { get; set; }
}

/// <summary>
/// The application of an OAuth2 authorization.
/// </summary>
public class OAuthAuthorizationApplicationJson
{
    /// <summary>
    /// The application id.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The application name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The icon hash.
    /// </summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The application description.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether anyone can invite the bot.
    /// </summary>
    [JsonProperty("bot_public")]
    public bool IsPublic { get; set; }
}

/// <summary>
/// The outcome of granting consent to an OAuth2 authorization.
/// </summary>
public class OAuthConsentJson
{
    /// <summary>
    /// The URL to send the user to next.
    /// </summary>
    [JsonProperty("redirect_to")]
    public string RedirectTo { get; set; }
}
