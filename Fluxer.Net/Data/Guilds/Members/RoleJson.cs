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
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<@&{Id}>";

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("permissions")]
    public GuildPermissions Permissions { get; set; }

    /// <inheritdoc />
    [JsonProperty("position")]
    public int Position { get; set; }

    /// <inheritdoc />
    [JsonProperty("hoist_position")]
    public int? HoistPosition { get; set; }

    /// <inheritdoc />
    [JsonProperty("color")]
    public int Color { get; set; }

    /// <inheritdoc />
    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    /// <summary>
    /// Get the role icon url, or null when the role has no icon image.
    /// </summary>
    /// <param name="size">Image size between 16 and 512.</param>
    public string? GetIconUrl(int size = 64)
        => string.IsNullOrEmpty(IconHash) ? null : $"https://fluxerusercontent.com/role-icons/{Id}/{IconHash}.webp?size={size}";

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
