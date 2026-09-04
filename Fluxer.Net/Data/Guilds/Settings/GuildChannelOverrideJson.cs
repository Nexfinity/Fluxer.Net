using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildChannelOverrideJson : IGuildChannelOverride
{
    /// <inheritdoc />
    [JsonProperty("collapsed")]
    public bool Collapsed { get; set; }

    /// <inheritdoc />
    [JsonProperty("message_notifications")]
    public int? MessageNotifications { get; set; }

    /// <inheritdoc />
    [JsonProperty("muted")]
    public bool Muted { get; set; }

    /// <inheritdoc />
    [JsonProperty("mute_config")]
    public MuteConfigurationJson? MuteConfig { get; set; }

    IMuteConfiguration? IGuildChannelOverride.MuteConfig => MuteConfig;
}