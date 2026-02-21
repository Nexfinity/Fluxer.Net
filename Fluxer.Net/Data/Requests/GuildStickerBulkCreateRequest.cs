using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildRequestSchemas.tsx#L235C14-L235C43"/>
/// </remarks>
public class GuildStickerBulkCreateRequest
{
    [MinLength(1)]
    [MaxLength(50)]
    [JsonPropertyName("stickers")]
    public GuildStickerCreateRequest[] Stickers { get; set; } = Array.Empty<GuildStickerCreateRequest>();
}
