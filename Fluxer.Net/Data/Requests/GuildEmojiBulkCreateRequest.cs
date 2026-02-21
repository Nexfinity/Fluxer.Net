using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class GuildEmojiBulkCreateRequest
{
    [MinLength(1)]
    [MaxLength(50)]
    [JsonPropertyName("emojis")]
    public GuildEmojiCreateRequest[] Emojis { get; set; } = Array.Empty<GuildEmojiCreateRequest>();
}
