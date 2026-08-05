using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for GUILD_EMOJIS_UPDATE event when guild emojis are updated.
/// </summary>
public class GuildEmojisUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("emojis")]
    public List<GuildEmojiJson> Emojis { get; set; } = new();
}
