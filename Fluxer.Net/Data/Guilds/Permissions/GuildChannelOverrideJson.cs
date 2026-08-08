using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildChannelOverrideJson
{
    [JsonProperty("collapsed")]
    public bool Collapsed { get; set; }

    [JsonProperty("message_notifications")]
    public int? MessageNotifications { get; set; }

    [JsonProperty("muted")]
    public bool Muted { get; set; }

    [JsonProperty("mute_config")]
    public MuteConfigurationJson? MuteConfig { get; set; }
}
