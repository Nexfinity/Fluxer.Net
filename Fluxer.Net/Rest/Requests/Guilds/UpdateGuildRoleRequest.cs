using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// 
/// </summary>
public class UpdateGuildRoleRequest
{
    /// <summary>
    /// The color of the role as an integer
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// Whether the role should be displayed separately in the member list
    /// </summary>
    [JsonProperty("hoist")]
    public bool? Hoist { get; set; }

    /// <summary>
    /// The position of the role in the hoisted member list
    /// </summary>
    [JsonProperty("hoist_position")]
    public int? HoistPosition { get; set; }

    /// <summary>
    /// Whether the role can be mentioned by anyone
    /// </summary>
    [JsonProperty("mentionable")]
    public bool? Mentionable { get; set; }

    /// <summary>
    /// The name of the role (1-100 characters)
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("permissions")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong? Permissions { get; set; }

    /// <summary>
    /// Base64 data URI for the role icon image. Null removes the icon. Cannot be combined with <see cref="UnicodeEmoji"/>.
    /// </summary>
    [JsonProperty("icon")]
    public Optional<string?> IconBase64 { get; set; }

    /// <summary>
    /// A single unicode emoji to use as the role icon. Null removes it.
    /// </summary>
    [JsonProperty("unicode_emoji")]
    public Optional<string?> UnicodeEmoji { get; set; }
}
