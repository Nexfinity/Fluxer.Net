using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The public details of the guild an emoji or sticker belongs to.
/// </summary>
public class ExpressionSourceGuildJson
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// The name of the guild.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The icon hash of the guild.
    /// </summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The guild's features, such as VERIFIED, PARTNERED or DISCOVERABLE.
    /// </summary>
    [JsonProperty("features")]
    public string[] Features { get; set; } = Array.Empty<string>();
}
