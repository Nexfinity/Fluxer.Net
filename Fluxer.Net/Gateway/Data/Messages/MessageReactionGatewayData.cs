using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Messages;

public class MessageReactionGatewayData
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
    public ReactionEmoji Emoji { get; set; }
}

public class ReactionEmoji
{
    [JsonProperty("id")]
    public ulong? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("animated")]
    public bool? Animated { get; set; }
}
