using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

public class BulkCreateGuildStickersRequest
{
    [MinLength(1)]
    [MaxLength(50)]
    [JsonProperty("stickers")]
    public CreateGuildStickerRequest[] Stickers { get; set; } = Array.Empty<CreateGuildStickerRequest>();
}
