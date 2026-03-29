using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/38146cc2babb504bfa9e71f61a60dd57ab2c1b67/packages/schema/src/domains/guild/GuildRequestSchemas.tsx#L235C14-L235C43"/>
/// </remarks>
public class BulkCreateGuildStickersRequest
{
    [MinLength(1)]
    [MaxLength(50)]
    [JsonProperty("stickers")]
    public CreateGuildStickerRequest[] Stickers { get; set; } = Array.Empty<CreateGuildStickerRequest>();
}
