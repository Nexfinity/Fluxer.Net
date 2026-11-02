using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a voice channel in a guild.
/// </summary>
public class CreateVoiceChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a voice channel.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildVoice;

    /// <summary>
    /// The bitrate of the voice channel in bits per second.
    /// </summary>
    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    /// <summary>
    /// The maximum number of users allowed in the voice channel.
    /// </summary>
    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    /// <summary>
    /// The maximum number of connections a user can have in the voice channel.
    /// </summary>
    [JsonProperty("voice_connection_limit")]
    public int? VoiceConnectionLimit { get; set; }
}