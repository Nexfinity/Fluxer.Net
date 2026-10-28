using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for updating a guild, sending only the properties that are set.
/// </summary>
public class UpdateGuildRequest
{
    [JsonProperty("name")]
    public Optional<string> Name { get; set; }

    [JsonProperty("icon")]
    public Optional<string?> IconBase64 { get; set; }

    [JsonProperty("banner")]
    public Optional<string?> BannerBase64 { get; set; }

    [JsonProperty("splash")]
    public Optional<string?> SplashBase64 { get; set; }

    [JsonProperty("system_channel_id")]
    public Optional<ulong?> SystemChannelId { get; set; }

    [JsonProperty("system_channel_flags")]
    public Optional<SystemChannelFlags> SystemChannelFlags { get; set; }

    [JsonProperty("afk_channel_id")]
    public Optional<ulong?> AfkChannelId { get; set; }

    [JsonProperty("afk_timeout")]
    public Optional<int> AfkTimeout { get; set; }

    [JsonProperty("default_message_notifications")]
    public Optional<GuildDefaultNotifications> DefaultMessageNotifications { get; set; }

    [JsonProperty("verification_level")]
    public Optional<GuildVerificationLevel> VerificationLevel { get; set; }

    [JsonProperty("explicit_content_filter")]
    public Optional<GuildContentFilter> ExplicitContentFilter { get; set; }

    public void IconFromStream(Stream stream) => IconBase64 = ToBase64(stream);

    public void BannerFromStream(Stream stream) => BannerBase64 = ToBase64(stream);

    public void SplashFromStream(Stream stream) => SplashBase64 = ToBase64(stream);

    private static string ToBase64(Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return Convert.ToBase64String(ms.ToArray());
    }
}
