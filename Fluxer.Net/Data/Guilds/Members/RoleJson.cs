using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class RoleJson : IRole
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }


    /// <inheritdoc />
    [JsonProperty("permissions")]
    [JsonConverter(typeof(Extensions.StringUInt64Converter))]
    public ulong Permissions { get; set; }

    /// <inheritdoc />
    [JsonProperty("position")]
    public int Position { get; set; }

    /// <inheritdoc />
    [JsonProperty("color")]
    public int Color { get; set; }

    /// <inheritdoc />
    [JsonProperty("unicode_emoji")]
    public string? UnicodeEmoji { get; set; }

    /// <inheritdoc />
    [JsonProperty("hoist")]
    public bool IsHoisted { get; set; }

    /// <inheritdoc />
    [JsonProperty("mentionable")]
    public bool IsMentionable { get; set; }
}
