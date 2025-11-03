using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class MessageReactionRemoveEmojiGatewayData : IGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("emoji")]
    public ReactionEmoji Emoji { get; set; }
}
