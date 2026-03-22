using Fluxer.Net.Gateway.Data.Voice;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class PassiveGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channels")]
    public Dictionary<ulong, ulong> Channels { get; set; }

    [JsonProperty("voice_states")]
    public VoiceStateGatewayData[]? VoiceStates { get; set; }

    [JsonProperty("created_channels")]
    public ChannelJson[]? CreatedChannels { get; set; }

    [JsonProperty("updated_channels")]
    public ChannelJson[]? UpdatedChannels { get; set; }

    [JsonProperty("deleted_channel_ids")]
    public ulong[] DeletedChannelIds { get; set; }
}
