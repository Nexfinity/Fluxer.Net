using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class RequestGuildChannelsPacket
{
    [JsonProperty("guild_id")]
    public string GuildId { get; set; }

    [JsonProperty("channel_ids")]
    public string[] ChannelIds { get; set; }
}
