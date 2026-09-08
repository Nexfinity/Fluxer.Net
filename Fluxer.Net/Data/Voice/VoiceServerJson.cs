using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class VoiceServerJson : IVoiceServer
{
    /// <inheritdoc />
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <inheritdoc />
    [JsonProperty("endpoint")]
    public string Endpoint { get; set; }

    /// <inheritdoc />
    [JsonProperty("connection_id")]
    public string ConnectionId { get; set; }

    /// <inheritdoc />
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <inheritdoc />
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    [JsonProperty("e2ee_key")]
    public string? E2EEKey { get; set; }
}
