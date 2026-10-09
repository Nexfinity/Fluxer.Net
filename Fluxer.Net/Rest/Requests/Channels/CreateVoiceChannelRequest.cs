using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreateVoiceChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildVoice;

    [JsonProperty("bitrate")]
    public int? Bitrate { get; set; }

    [JsonProperty("user_limit")]
    public int? UserLimit { get; set; }

    [JsonProperty("voice_connection_limit")]
    public int? VoiceConnectionLimit { get; set; }
}