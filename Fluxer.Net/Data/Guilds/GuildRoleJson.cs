using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildRoleJson
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The role's permission bitfield. Sent as a quoted string by the gateway (e.g. "8933636165184").
    /// </summary>
    [JsonProperty("permissions")]
    [JsonConverter(typeof(Extensions.StringUInt64Converter))]
    public ulong Permissions { get; set; }

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("color")]
    public int Color { get; set; }

    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    [JsonProperty("unicode_emoji")]
    public string? UnicodeEmoji { get; set; }

    [JsonProperty("hoist")]
    public bool IsHoisted { get; set; }

    [JsonProperty("mentionable")]
    public bool IsMentionable { get; set; }
}
