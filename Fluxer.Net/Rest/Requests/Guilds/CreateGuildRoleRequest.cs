using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// <para>Request body for creating a role in a guild.</para>
/// </summary>
public class CreateGuildRoleRequest
{
    /// <summary>
    /// Color as an integer (e.g: 0xff0000 for red)
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// The name of the role (1-100 characters)
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

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

    /// <summary>
    /// The ID of a role in the same guild whose icon the new role copies. Cannot be combined with an icon or emoji.
    /// </summary>
    [JsonProperty("icon_source_role_id")]
    public ulong? IconSourceRoleId { get; set; }
}
