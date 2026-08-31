using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class VoiceStateJson : IVoiceState
{
    /// <inheritdoc />
    [JsonProperty("session_id")]
    public string SessionId { get; set; }

    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("connection_id")]
    public string? ConnectionId { get; set; }

    /// <inheritdoc />
    [JsonProperty("deaf")]
    public bool IsDeaf { get; set; }

    /// <inheritdoc />
    [JsonProperty("mute")]
    public bool IsMute { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("is_mobile")]
    public bool IsMobile { get; set; }

    /// <inheritdoc />
    [JsonProperty("self_deaf")]
    public bool IsSelfDeaf { get; set; }

    /// <inheritdoc />
    [JsonProperty("self_mute")]
    public bool IsSelfMute { get; set; }

    /// <inheritdoc />
    [JsonProperty("self_stream")]
    public bool IsSelfStream { get; set; }

    /// <inheritdoc />
    [JsonProperty("self_video")]
    public bool IsSelfVideo { get; set; }

    /// <inheritdoc />
    [JsonProperty("viewer_stream_keys")]
    public string[] ViewerStreamKeys { get; set; }

    /// <inheritdoc />
    [JsonProperty("member")]
    public GuildMemberJson Member { get; set; }

    IGuildMember IVoiceState.Member => Member;
}
