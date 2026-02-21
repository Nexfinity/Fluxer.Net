using Fluxer.Net.Gateway.Data;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildEmojiSchemas.tsx#L60"/>
/// </remarks>
public class GuildStickerWithUserResponse : GuildStickerResponse
{
    /// <summary>
    /// User that created this sticker
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("user")]
    public UserPartialResponse User { get; set; }
}
