using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fluxer.Net;

public class BulkCreateGuildEmojisRequest
{
    [MinLength(1)]
    [MaxLength(50)]
    [JsonProperty("emojis")]
    public CreateGuildEmojiRequest[] Emojis { get; set; } = Array.Empty<CreateGuildEmojiRequest>();
}
