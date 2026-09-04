using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class UserGuildSettingsJson : IUserGuildSettings
{
    /// <inheritdoc />
    [JsonProperty("channel_overrides")]
    public Dictionary<ulong, GuildChannelOverrideJson>? ChannelOverrides { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("hide_muted_channels")]
    public bool HideMutedChannels { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_notifications")]
    public NotificationType MessageNotifications { get; set; }

    /// <inheritdoc />
    [JsonProperty("mobile_push")]
    public bool MobilePush { get; set; }

    /// <inheritdoc />
    [JsonProperty("mute_config")]
    public MuteConfigurationJson? MuteConfig { get; set; }

    /// <inheritdoc />
    [JsonProperty("muted")]
    public bool Muted { get; set; }

    /// <inheritdoc />
    [JsonProperty("suppress_everyone")]
    public bool SuppressEveryone { get; set; }

    /// <inheritdoc />
    [JsonProperty("suppress_roles")]
    public bool SuppressRoles { get; set; }

    /// <inheritdoc />
    [JsonProperty("version")]
    public int Version { get; set; }

    IDictionary<ulong, IGuildChannelOverride>? IUserGuildSettings.ChannelOverrides => (IDictionary<ulong, IGuildChannelOverride>?)ChannelOverrides;

    IMuteConfiguration? IUserGuildSettings.MuteConfig => MuteConfig;
}
