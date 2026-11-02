using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the guild.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// The name of the guild.
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The id of the guild owner.
    /// </summary>
    [JsonProperty("owner_id")]
    public ulong? OwnerId { get; set; }

    /// <summary>
    /// The vanity invite code of the guild.
    /// </summary>
    [JsonProperty("vanity_url_code")]
    public string? VanityUrl { get; set; }

    /// <summary>
    /// The icon hash of the guild.
    /// </summary>
    [JsonProperty("icon_hash")]
    public string? IconHash { get; set; }

    /// <summary>
    /// The banner hash of the guild.
    /// </summary>
    [JsonProperty("banner_hash")]
    public string? BannerHash { get; set; }

    /// <summary>
    /// The invite splash hash of the guild.
    /// </summary>
    [JsonProperty("splash_hash")]
    public string? SplashHash { get; set; }

    /// <summary>
    /// The width of the invite splash.
    /// </summary>
    [JsonProperty("splash_width")]
    public int? SplashWidth { get; set; }

    /// <summary>
    /// The height of the invite splash.
    /// </summary>
    [JsonProperty("splash_height")]
    public int? SplashHeight { get; set; }

    /// <summary>
    /// The alignment of the invite splash card.
    /// </summary>
    [JsonProperty("splash_card_alignment")]
    public GuildSplashCardAlignment? SplashCardAligment { get; set; }

    /// <summary>
    /// The embed splash hash of the guild.
    /// </summary>
    [JsonProperty("embed_splash")]
    public string? EmbedSplashHash { get; set; }

    /// <summary>
    /// The width of the embed splash.
    /// </summary>
    [JsonProperty("embed_splash_width")]
    public int? EmbedSplashWidth { get; set; }

    /// <summary>
    /// The height of the embed splash.
    /// </summary>
    [JsonProperty("embed_splash_height")]
    public int? EmbedSplashHeight { get; set; }

    /// <summary>
    /// The features enabled for the guild.
    /// </summary>
    [JsonProperty("features")]
    public string[]? Features { get; set; }

    /// <summary>
    /// The verification level of the guild.
    /// </summary>
    [JsonProperty("verification_level")]
    public GuildVerificationLevel? VerificationLevel { get; set; }

    /// <summary>
    /// The multi-factor authentication level of the guild.
    /// </summary>
    [JsonProperty("mfa_level")]
    public GuildMfaLevel? MfaLevel { get; set; }

    /// <summary>
    /// The NSFW level of the guild.
    /// </summary>
    [JsonProperty("nsfw_level")]
    public GuildNsfwLevel? NsfwLevel { get; set; }

    /// <summary>
    /// The content warning level of the guild.
    /// </summary>
    [JsonProperty("content_warning_level")]
    public GuildContentWarning? ContentWarningLevel { get; set; }

    /// <summary>
    /// The content warning text of the guild.
    /// </summary>
    [JsonProperty("content_warning_text")]
    public string? ContentWarningText { get; set; }

    /// <summary>
    /// The explicit content filter level of the guild.
    /// </summary>
    [JsonProperty("explicit_content_filter")]
    public GuildContentFilter ExplicitContentFilter { get; set; }

    /// <summary>
    /// The default notification level of the guild.
    /// </summary>
    [JsonProperty("default_message_notifications")]
    public GuildDefaultNotifications? DefaultMessageNotifications { get; set; }

    /// <summary>
    /// The id of the system channel.
    /// </summary>
    [JsonProperty("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    /// <summary>
    /// The flags of the system channel.
    /// </summary>
    [JsonProperty("system_channel_flags")]
    public SystemChannelFlags? SystemChannelFlags { get; set; }

    /// <summary>
    /// The id of the rules channel.
    /// </summary>
    [JsonProperty("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    /// <summary>
    /// The id of the AFK channel.
    /// </summary>
    [JsonProperty("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    /// <summary>
    /// The time in seconds an idle member waits before being moved to the AFK channel.
    /// </summary>
    [JsonProperty("afk_timeout")]
    public int? AfkTimeout { get; set; }

    /// <summary>
    /// The operations disabled for the guild.
    /// </summary>
    [JsonProperty("disabled_operations")]
    public GuildOperations? DisabledOperations { get; set; }

    /// <summary>
    /// The time before which messages are not visible to members without Read Message History.
    /// </summary>
    [JsonProperty("message_history_cutoff")]
    public DateTimeOffset? MessageHistoryCutoff { get; set; }
}
