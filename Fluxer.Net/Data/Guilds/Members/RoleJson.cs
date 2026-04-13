using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class RoleJson : IRole
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<@&{Id}>";

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("permissions")]
    public Permissions Permissions { get; set; }

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
