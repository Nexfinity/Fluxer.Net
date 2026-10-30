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

    /// <summary>
    /// The complete desired feature set. Only user-toggleable features may differ from the current set, the others must be kept as they are.
    /// </summary>
    [JsonProperty("features")]
    public Optional<string[]> Features { get; set; }

    /// <summary>
    /// Required MFA level for moderation actions. Changing it needs sudo verification.
    /// </summary>
    [JsonProperty("mfa_level")]
    public Optional<GuildMfaLevel> MfaLevel { get; set; }

    /// <summary>
    /// Whether the guild is marked as adult (18+) content.
    /// </summary>
    [JsonProperty("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    /// <summary>
    /// Whether the guild displays a content warning before entering.
    /// </summary>
    [JsonProperty("content_warning_level")]
    public Optional<GuildContentWarning> ContentWarningLevel { get; set; }

    /// <summary>
    /// Custom guild-wide content warning text (max 200 characters). Null falls back to a localized default.
    /// </summary>
    [JsonProperty("content_warning_text")]
    public Optional<string?> ContentWarningText { get; set; }

    /// <summary>
    /// Base64 image data for the embedded invite splash.
    /// </summary>
    [JsonProperty("embed_splash")]
    public Optional<string?> EmbedSplashBase64 { get; set; }

    /// <summary>
    /// Alignment of the splash card.
    /// </summary>
    [JsonProperty("splash_card_alignment")]
    public Optional<GuildSplashCardAlignment> SplashCardAlignment { get; set; }

    /// <summary>
    /// How far back members without Read Message History can access messages. Null disables historical access.
    /// </summary>
    [JsonIgnore]
    public Optional<DateTimeOffset?> MessageHistoryCutoff { get; set; }

    [JsonProperty("message_history_cutoff")]
    private Optional<string?> MessageHistoryCutoffIso
        => MessageHistoryCutoff.HasValue
            ? Optional.Some<string?>(MessageHistoryCutoff.Value?.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.CultureInfo.InvariantCulture))
            : default;

    /// <summary>
    /// Account password for sudo verification.
    /// </summary>
    [JsonProperty("password")]
    public string? Password { get; set; }

    /// <summary>
    /// "totp", "webauthn" or "email_code".
    /// </summary>
    [JsonProperty("mfa_method")]
    public string? MfaMethod { get; set; }

    /// <summary>
    /// MFA verification code from an authenticator app.
    /// </summary>
    [JsonProperty("mfa_code")]
    public string? MfaCode { get; set; }

    /// <summary>
    /// Ticket from the sudo email code endpoint, sent with the "email_code" method.
    /// </summary>
    [JsonProperty("email_code_ticket")]
    public string? EmailCodeTicket { get; set; }

    /// <summary>
    /// The 6-digit code from the email, sent with the "email_code" method.
    /// </summary>
    [JsonProperty("email_code")]
    public string? EmailCode { get; set; }

    /// <summary>
    /// WebAuthn authentication response.
    /// </summary>
    [JsonProperty("webauthn_response")]
    public object? WebAuthnResponse { get; set; }

    /// <summary>
    /// WebAuthn challenge string.
    /// </summary>
    [JsonProperty("webauthn_challenge")]
    public string? WebAuthnChallenge { get; set; }

    /// <summary>
    /// Set the embedded invite splash from an image stream.
    /// </summary>
    public void EmbedSplashFromStream(Stream stream) => EmbedSplashBase64 = ToBase64(stream);

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
