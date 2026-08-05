using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for updating the current user's voice state.
/// Sent as part of VOICE_STATE_UPDATE opcode (4).
/// </summary>
public class VoiceStateUpdatePayload
{
    [JsonProperty("guild_id")]
    public string GuildId { get; set; }

    [JsonProperty("channel_id")]
    public string? ChannelId { get; set; }

    [JsonProperty("connection_id")]
    public string? ConnectionId { get; set; }

    [JsonProperty("self_mute")]
    public bool SelfMute { get; set; }

    [JsonProperty("self_deaf")]
    public bool SelfDeaf { get; set; }

    [JsonProperty("self_stream")]
    public bool SelfStream { get; set; }

    [JsonProperty("self_video")]
    public bool SelfVideo { get; set; }

    [JsonProperty("is_mobile")]
    public bool IsMobile { get; set; }

    [JsonProperty("latitude")]
    public string? Latitude { get; set; }

    [JsonProperty("longitude")]
    public string? Longitude { get; set; }

    public VoiceStateUpdatePayload(string? guildId, string? channelId, bool selfMute, bool selfDeaf)
    {
        GuildId = guildId;
        ChannelId = channelId;
        SelfMute = selfMute;
        SelfDeaf = selfDeaf;
        SelfStream = false;
        SelfVideo = false;
        IsMobile = false;
        ConnectionId = null;
        Latitude = null;
        Longitude = null;
    }
}