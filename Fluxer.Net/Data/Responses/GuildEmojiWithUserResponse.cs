using Fluxer.Net.Gateway.Data;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

public class GuildEmojiWithUserResponse : GuildEmojiResponse
{
    /// <summary>
    /// User that created the emoji
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("user")]
    public UserPartialResponse User { get; set; }
}
