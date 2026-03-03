using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class GuildEmojiWithUserResponse : GuildEmojiResponse
{
    /// <summary>
    /// User that created the emoji
    /// </summary>
    [JsonRequired]
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
