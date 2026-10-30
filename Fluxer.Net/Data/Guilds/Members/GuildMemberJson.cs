using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildMemberJson : IGuildMember
{
    /// <inheritdoc />
    [JsonIgnore]
    public ulong Id => User.Id;

    /// <inheritdoc />
    [JsonIgnore]
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    [JsonIgnore]
    public string Mention => $"<@{Id}>";

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson User { get; set; }

    /// <inheritdoc />
    [JsonProperty("joined_at")]
    public DateTimeOffset JoinedAt { get; set; }

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
    public DateTimeOffset? CommunicationDisabledUntil { get; set; }

    /// <inheritdoc />
    [JsonProperty("roles")]
    public HashSet<ulong> RoleIds { get; set; } = new HashSet<ulong>();

    /// <summary>
    /// Flags on the member's guild profile.
    /// </summary>
    [JsonProperty("profile_flags")]
    public GuildMemberProfileFlags? ProfileFlags { get; set; }

    /// <summary>
    /// Whether replies to the member mention them by default in this guild.
    /// </summary>
    [JsonProperty("mention_flags")]
    public MentionReplyPreference? MentionFlags { get; set; }

    /// <inheritdoc />
    public string GetCurrentName()
    {
        return Nickname ?? User.DisplayName ?? User.Username;
    }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{Id % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        return GetGuildAvatarUrl(size) ?? User.GetAvatarUrl(size);
    }

    /// <inheritdoc />
    public string? GetGuildAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return null;

        return $"https://fluxerusercontent.com/guilds/{GuildId}/users/{Id}/avatars/{AvatarHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string GetAvatarOrDefaultUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash) && string.IsNullOrEmpty(User.AvatarHash))
            return GetDefaultAvatarUrl();

        return GetAvatarUrl(size);
    }

    /// <inheritdoc />
    public string? GetBannerUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"https://fluxerusercontent.com/guilds/{GuildId}/users/{Id}/banners/{BannerHash}.webp?size={size}";
    }

    IUser IGuildMember.User => User;
}
