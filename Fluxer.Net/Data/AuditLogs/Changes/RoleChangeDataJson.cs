using Newtonsoft.Json;

namespace Fluxer.Net;

public class RoleChangeDataJson : IAuditLogData
{
    [JsonProperty("role_id")]
    public ulong RoleId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("permissions")]
    public GuildPermissions? Permissions { get; set; }

    [JsonProperty("position")]
    public int? Position { get; set; }

    [JsonProperty("hoist_position")]
    public int? HoistPosition { get; set; }

    [JsonProperty("color")]
    public int? Color { get; set; }

    [JsonProperty("unicode_emoji")]
    public string? UnicodeEmoji { get; set; }

    [JsonProperty("hoist")]
    public bool? IsHoisted { get; set; }

    [JsonProperty("mentionable")]
    public bool? IsMentionable { get; set; }

    // permissions_diff
}
