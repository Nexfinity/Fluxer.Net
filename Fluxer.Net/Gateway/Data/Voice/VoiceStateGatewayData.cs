using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Voice;

public class VoiceStateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("connection_id")]
    public string ConnectionId { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    [JsonProperty("member")]
    public GuildMemberJson Member { get; set; }

    [JsonProperty("deaf")]
    public bool Deaf { get; set; }

    [JsonProperty("mute")]
    public bool Mute { get; set; }

    [JsonProperty("self_deaf")]
    public bool SelfDeaf { get; set; }

    [JsonProperty("self_mute")]
    public bool SelfMute { get; set; }

    [JsonProperty("self_video")]
    public bool SelfVideo { get; set; }

    [JsonProperty("self_stream")]
    public bool? SelfStream { get; set; }

    [JsonProperty("is_mobile")]
    public bool IsMobile { get; set; }

    [JsonProperty("viewer_stream_keys")]
    public string[]? ViewerStreamKeys { get; set; }
}
