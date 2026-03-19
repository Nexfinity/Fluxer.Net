using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildEmojiSchemas.tsx#L60"/>
/// </remarks>
public class GuildStickerWithUserJson : GuildStickerResponse
{
    /// <summary>
    /// User that created this sticker
    /// </summary>
    [JsonRequired]
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
