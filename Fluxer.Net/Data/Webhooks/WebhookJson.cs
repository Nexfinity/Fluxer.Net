using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class WebhookJson : IWebhook
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("token")]
    public string? Token { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson? Creator { get; set; }

    /// <inheritdoc />
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <inheritdoc />
    [JsonProperty("avatar")]
    public string? AvatarHash { get; set; }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{Id % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return null;

        return $"https://fluxerusercontent.com/avatars/{Id}/{AvatarHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string GetAvatarOrDefaultUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return GetDefaultAvatarUrl();

        return GetAvatarUrl(size);
    }
}
