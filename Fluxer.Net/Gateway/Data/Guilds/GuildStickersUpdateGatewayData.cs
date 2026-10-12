using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class GuildStickersUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("stickers")]
    public List<GuildStickerJson> Stickers { get; set; } = new();
}