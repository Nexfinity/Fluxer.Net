using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildMemberJson : IGuildMember
{
    /// <inheritdoc />
    [JsonIgnore]
    public ulong UserId => User.Id;

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<@{UserId}>";

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson User { get; set; }

    /// <inheritdoc />
    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("nick")]
    public string? Nickname { get; set; }

    /// <inheritdoc />
    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    /// <inheritdoc />
    [JsonProperty("accent_color")]
    public int? AccentColor { get; set; }

    /// <inheritdoc />
    [JsonProperty("deaf")]
    public bool IsDeaf { get; set; }

    /// <inheritdoc />
    [JsonProperty("mute")]
    public bool IsMute { get; set; }

    /// <inheritdoc />
    [JsonProperty("communication_disabled_until")]
    public DateTime? CommunicationDisabledUntil { get; set; }

    /// <inheritdoc />
    [JsonProperty("roles")]
    public HashSet<ulong>? RoleIds { get; set; }

    /// <inheritdoc />
    public string GetCurrentName()
    {
        return Nickname ?? User.DisplayName ?? User.Username;
    }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{UserId % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return User.GetAvatarUrl();

        return $"https://fluxerusercontent.com/avatars/{UserId}/{AvatarHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string GetAvatarOrDefaultUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash) && string.IsNullOrEmpty(User.AvatarHash))
            return GetDefaultAvatarUrl();

        return GetAvatarUrl(size);
    }

    IUser IGuildMember.User => User;

}
