using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildJson : PartialGuildJson, IGuild
{
    /// <inheritdoc />
    [JsonProperty("owner_id")]
    public ulong OwnerId { get; set; }

    /// <inheritdoc />
    [JsonProperty("vanity_url_code")]
    public string? VanityUrlCode { get; set; }

    /// <inheritdoc />
    [JsonProperty("verification_level")]
    public GuildVerificationLevel VerificationLevel { get; set; }

    /// <inheritdoc />
    [JsonProperty("mfa_level")]
    public GuildMfaLevel MfaLevel { get; set; }

    /// <inheritdoc />
    [JsonProperty("nsfw_level")]
    public GuildNsfwLevel NsfwLevel { get; set; }

    /// <inheritdoc />
    [JsonProperty("explicit_content_filter")]
    public GuildContentFilter ExplicitContentFilter { get; set; }

    /// <inheritdoc />
    [JsonProperty("default_message_notifications")]
    public GuildDefaultNotifications DefaultMessageNotifications { get; set; }

    /// <inheritdoc />
    [JsonProperty("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("system_channel_flags")]
    public SystemChannelFlags SystemChannelFlags { get; set; }

    /// <inheritdoc />
    [JsonProperty("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("afk_timeout")]
    public int AfkTimeout { get; set; }

    /// <inheritdoc />
    [JsonProperty("disabled_operations")]
    public ulong DisabledOperations { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_history_cutoff")]
    public DateTime? MessageHistoryCutoff { get; set; }
}
