using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class ChannelCreateVoiceRequest : ChannelCreateRequest
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    // NOTE bitrate and user limit is only respected for voice request
    // https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/api/src/guild/services/channel/ChannelOperationsService.tsx#L159-L160

    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; set; }

    [JsonPropertyName("user_limit")]
    public int? UserLimit { get; set; }
}
