using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class MessageReactionGatewayData
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("emoji")]
    public EmojiJson Emoji { get; set; }

    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }
}