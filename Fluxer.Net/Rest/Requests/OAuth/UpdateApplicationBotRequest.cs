using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Changes the bot user of an application. Only the fields that are set change.
/// </summary>
public class UpdateApplicationBotRequest
{
    /// <summary>
    /// The bot username.
    /// </summary>
    [JsonProperty("username")]
    public string? Username { get; set; }

    /// <summary>
    /// The bot discriminator.
    /// </summary>
    [JsonProperty("discriminator")]
    public string? Discriminator { get; set; }

    /// <summary>
    /// The avatar as a base64 image data URI.
    /// </summary>
    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// The banner as a base64 image data URI.
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    /// <summary>
    /// The bot bio, up to 1024 characters.
    /// </summary>
    [JsonProperty("bio")]
    public string? Bio { get; set; }

    /// <summary>
    /// The bot user flags.
    /// </summary>
    [JsonProperty("bot_flags")]
    public int? BotFlags { get; set; }
}
