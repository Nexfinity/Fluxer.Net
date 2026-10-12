using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildEmojisUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("emojis")]
    public List<GuildEmojiJson> Emojis { get; set; } = new();
}