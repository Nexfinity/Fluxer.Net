using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class UserJson : IUser
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<@{Id}>";

    /// <inheritdoc />
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <inheritdoc />
    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    /// <inheritdoc />
    [JsonProperty("global_name")]
    public string? DisplayName { get; set; }

    /// <inheritdoc />
    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("avatar_color")]
    public int? AvatarColor { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public UserFlags Flags { get; set; }

    /// <inheritdoc />
    [JsonProperty("bot")]
    public bool IsBot { get; set; }

    /// <inheritdoc />
    [JsonProperty("system")]
    public bool IsSystem { get; set; }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{Id % 6}.png";
    }
}
