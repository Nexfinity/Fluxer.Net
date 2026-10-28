using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The public details of the guild an emoji or sticker belongs to.
/// </summary>
public class ExpressionSourceGuildJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The guild's badges, limited to VERIFIED, PARTNERED and DISCOVERABLE.
    /// </summary>
    [JsonProperty("features")]
    public string[] Features { get; set; } = Array.Empty<string>();
}
