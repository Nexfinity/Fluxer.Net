using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildChangeDataJson : IAuditLogData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("owner_id")]
    public ulong? OwnerId { get; set; }

    [JsonProperty("vanity_url_code")]
    public string? VanityUrl { get; set; }

    [JsonProperty("icon_hash")]
    public string? IconHash { get; set; }

    [JsonProperty("banner_hash")]
    public string? BannerHash { get; set; }

    [JsonProperty("splash_hash")]
    public string? SplashHash { get; set; }

    [JsonProperty("splash_width")]
    public int? SplashWidth { get; set; }

    [JsonProperty("splash_height")]
    public int? SplashHeight { get; set; }

    [JsonProperty("splash_card_alignment")]
    public GuildSplashCardAlignment? SplashCardAligment { get; set; }

    [JsonProperty("embed_splash")]
    public string? EmbedSplashHash { get; set; }

    [JsonProperty("embed_splash_width")]
    public int? EmbedSplashWidth { get; set; }

    [JsonProperty("embed_splash_height")]
    public int? EmbedSplashHeight { get; set; }

    [JsonProperty("features")]
    public string[]? Features { get; set; }

    [JsonProperty("verification_level")]
    public GuildVerificationLevel? VerificationLevel { get; set; }

    [JsonProperty("mfa_level")]
    public GuildMfaLevel? MfaLevel { get; set; }

    [JsonProperty("nsfw_level")]
    public GuildNsfwLevel? NsfwLevel { get; set; }

    [JsonProperty("content_warning_level")]
    public GuildContentWarning? ContentWarningLevel { get; set; }

    [JsonProperty("content_warning_text")]
    public string? ContentWarningText { get; set; }

    [JsonProperty("explicit_content_filter")]
    public GuildContentFilter ExplicitContentFilter { get; set; }

    [JsonProperty("default_message_notifications")]
    public GuildDefaultNotifications? DefaultMessageNotifications { get; set; }

    [JsonProperty("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonProperty("system_channel_flags")]
    public SystemChannelFlags? SystemChannelFlags { get; set; }

    [JsonProperty("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonProperty("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonProperty("afk_timeout")]
    public int? AfkTimeout { get; set; }

    [JsonProperty("disabled_operations")]
    public GuildOperations? DisabledOperations { get; set; }

    [JsonProperty("message_history_cutoff")]
    public DateTimeOffset? MessageHistoryCutoff { get; set; }
}
