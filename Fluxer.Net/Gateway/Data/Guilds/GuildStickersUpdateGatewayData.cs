using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data.Guilds;

/// <summary>
/// Gateway data for GUILD_STICKERS_UPDATE event when guild stickers are updated.
/// </summary>
public class GuildStickersUpdateGatewayData
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("stickers")]
    public List<GuildStickerJson> Stickers { get; set; } = new();
}
