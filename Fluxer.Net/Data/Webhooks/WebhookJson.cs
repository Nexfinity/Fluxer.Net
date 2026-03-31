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
}
