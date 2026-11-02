using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class RoleChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the role.
    /// </summary>
    [JsonProperty("role_id")]
    public ulong RoleId { get; set; }

    /// <summary>
    /// The name of the role.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The permissions of the role.
    /// </summary>
    [JsonProperty("permissions")]
    public GuildPermissions? Permissions { get; set; }

    /// <summary>
    /// The position of the role.
    /// </summary>
    [JsonProperty("position")]
    public int? Position { get; set; }

    /// <summary>
    /// The position of the role in the member list.
    /// </summary>
    [JsonProperty("hoist_position")]
    public int? HoistPosition { get; set; }

    /// <summary>
    /// The color of the role.
    /// </summary>
    [JsonProperty("color")]
    public int? Color { get; set; }

    /// <summary>
    /// The icon hash of the role.
    /// </summary>
    [JsonProperty("icon_hash")]
    public string? IconHash { get; set; }

    /// <summary>
    /// The unicode emoji of the role.
    /// </summary>
    [JsonProperty("unicode_emoji")]
    public string? UnicodeEmoji { get; set; }

    /// <summary>
    /// Whether the role is displayed separately in the member list.
    /// </summary>
    [JsonProperty("hoist")]
    public bool? IsHoisted { get; set; }

    /// <summary>
    /// Whether the role can be mentioned.
    /// </summary>
    [JsonProperty("mentionable")]
    public bool? IsMentionable { get; set; }

    // permissions_diff
}
