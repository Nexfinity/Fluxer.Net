using Newtonsoft.Json;

namespace Fluxer.Net.Data.Guilds;

public class Guild : Entity
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("vanity_url_code")]
    public string? VanityUrlCode { get; set; }

    [JsonProperty("icon")]
    public string? IconHash { get; set; }

    [JsonProperty("banner")]
    public string? BannerHash { get; set; }

    [JsonProperty("banner_width")]
    public int? BannerWidth { get; set; }

    [JsonProperty("banner_height")]
    public int? BannerHeight { get; set; }

    [JsonProperty("embed_splash")]
    public string? EmbedSplashHash { get; set; }

    [JsonProperty("embed_splash_width")]
    public int? EmbedSplashWidth { get; set; }

    [JsonProperty("embed_splash_height")]
    public int? EmbedSplashHeight { get; set; }

    [JsonProperty("splash")]
    public string? SplashHash { get; set; }

    [JsonProperty("splash_width")]
    public int? SplashWidth { get; set; }

    [JsonProperty("splash_height")]
    public int? SplashHeight { get; set; }

    [JsonProperty("splash_card_alignment")]
    public GuildSplashCardAlignment SplashCardAligment { get; set; }

    [JsonProperty("features")]
    public HashSet<string>? Features { get; set; }

    [JsonProperty("verification_level")]
    public int VerificationLevel { get; set; }

    [JsonProperty("mfa_level")]
    public int MfaLevel { get; set; }

    [JsonProperty("nsfw_level")]
    public int NsfwLevel { get; set; }

    [JsonProperty("explicit_content_filter")]
    public int ExplicitContentFilter { get; set; }

    [JsonProperty("default_message_notifications")]
    public int DefaultMessageNotifications { get; set; }

    [JsonProperty("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonProperty("system_channel_flags")]
    public int SystemChannelFlags { get; set; }

    [JsonProperty("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonProperty("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonProperty("afk_timeout")]
    public int AfkTimeout { get; set; }

    [JsonProperty("disabled_operations")]
    public int DisabledOperations { get; set; }

    [JsonProperty("max_presences")]
    public int? MaxPresences { get; set; }

    [JsonProperty("member_count")]
    public int MemberCount { get; set; }

    [JsonProperty("audit_logs_indexed_at")]
    public DateTime? AuditLogsIndexedAt { get; set; }

    [JsonProperty("message_history_cutoff")]
    public DateTime? MessageHistoryCutoff { get; set; }
}
