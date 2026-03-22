using Fluxer.Net.Gateway.Data.Messages;
using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildEmojiWithUserJson : GuildEmojiResponse
{
    /// <summary>
    /// User that created the emoji
    /// </summary>
    [JsonRequired]
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
