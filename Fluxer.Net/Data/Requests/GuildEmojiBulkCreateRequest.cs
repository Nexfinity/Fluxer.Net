using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

public class GuildEmojiBulkCreateRequest
{
    [MinLength(1)]
    [MaxLength(50)]
    [JsonProperty("emojis")]
    public GuildEmojiCreateRequest[] Emojis { get; set; } = Array.Empty<GuildEmojiCreateRequest>();
}
