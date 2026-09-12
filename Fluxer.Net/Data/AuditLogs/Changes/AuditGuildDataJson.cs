using Newtonsoft.Json;

namespace Fluxer.Net;

public class AuditGuildDataJson : IAuditLogData
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("afk_timeout")]
    public int? AfkTimeout { get; set; }

    [JsonProperty("widget_enabled")]
    public bool? IsEmbeddable { get; set; }

    //[JsonProperty("default_message_notifications")]
    //public DefaultMessageNotifications? DefaultMessageNotifications { get; set; }

    //[JsonProperty("mfa_level")]
    //public MfaLevel? MfaLevel { get; set; }

    //[JsonProperty("verification_level")]
    //public VerificationLevel? VerificationLevel { get; set; }

    //[JsonProperty("explicit_content_filter")]
    //public ExplicitContentFilterLevel? ExplicitContentFilterLevel { get; set; }

    [JsonProperty("icon_hash")]
    public string IconHash { get; set; }

    [JsonProperty("discovery_splash")]
    public string DiscoverySplash { get; set; }

    [JsonProperty("splash")]
    public string Splash { get; set; }

    [JsonProperty("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonProperty("widget_channel_id")]
    public ulong? EmbeddedChannelId { get; set; }

    [JsonProperty("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonProperty("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonProperty("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    [JsonProperty("owner_id")]
    public ulong? OwnerId { get; set; }

    [JsonProperty("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonProperty("region")]
    public string RegionId { get; set; }

    [JsonProperty("banner")]
    public string Banner { get; set; }

    [JsonProperty("vanity_url_code")]
    public string VanityUrl { get; set; }

    //[JsonProperty("system_channel_flags")]
    //public SystemChannelMessageDeny? SystemChannelFlags { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("preferred_locale")]
    public string PreferredLocale { get; set; }

    //[JsonProperty("nsfw_level")]
    //public NsfwLevel? NsfwLevel { get; set; }

    [JsonProperty("premium_progress_bar_enabled")]
    public bool? ProgressBarEnabled { get; set; }

}
