using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class MessageReactionAddManyGatewayData
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonProperty("reactions")]
    public List<MessageReactionAddManyEntryGatewayData> Reactions { get; set; } = new();
}

internal class MessageReactionAddManyEntryGatewayData
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("emoji")]
    public EmojiJson Emoji { get; set; }

    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }
}
