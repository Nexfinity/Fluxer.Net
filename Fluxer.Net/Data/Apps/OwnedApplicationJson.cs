using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// An application you own, as returned to its owner.
/// </summary>
public class OwnedApplicationJson
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
    /// The registered redirect URIs for OAuth2.
    /// </summary>
    [JsonProperty("redirect_uris")]
    public string[] RedirectUrls { get; set; }

    /// <summary>
    /// Whether anyone can invite the bot.
    /// </summary>
    [JsonProperty("bot_public")]
    public bool IsPublic { get; set; }

    /// <summary>
    /// Whether the bot requires the OAuth2 code grant.
    /// </summary>
    [JsonProperty("bot_require_code_grant")]
    public bool RequiresCodeGrant { get; set; }

    /// <summary>
    /// The client secret. Only sent when it was just created or reset.
    /// </summary>
    [JsonProperty("client_secret")]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// The bot user of the application.
    /// </summary>
    [JsonProperty("bot")]
    public ApplicationBotJson? Bot { get; set; }
}

/// <summary>
/// The bot user of an application you own.
/// </summary>
public class ApplicationBotJson
{
    /// <summary>
    /// The bot user id.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The bot username.
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// The bot discriminator.
    /// </summary>
    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    /// <summary>
    /// The avatar hash.
    /// </summary>
    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// The banner hash.
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// The bot bio.
    /// </summary>
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <summary>
    /// The bot token. Only sent when it was just reset.
    /// </summary>
    [JsonProperty("token")]
    public string? Token { get; set; }

    /// <summary>
    /// The bot user flags.
    /// </summary>
    [JsonProperty("flags")]
    public int Flags { get; set; }
}

/// <summary>
/// A bot token that was just reset.
/// </summary>
public class BotTokenResetJson
{
    /// <summary>
    /// The new bot token.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <summary>
    /// The bot user.
    /// </summary>
    [JsonProperty("bot")]
    public ApplicationBotJson Bot { get; set; }
}

/// <summary>
/// The profile of an application's bot after an update.
/// </summary>
public class BotProfileJson
{
    /// <summary>
    /// The bot user id.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The bot username.
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// The bot discriminator.
    /// </summary>
    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    /// <summary>
    /// The avatar hash.
    /// </summary>
    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// The banner hash.
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// The bot bio.
    /// </summary>
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <summary>
    /// The bot user flags.
    /// </summary>
    [JsonProperty("flags")]
    public int Flags { get; set; }
}
