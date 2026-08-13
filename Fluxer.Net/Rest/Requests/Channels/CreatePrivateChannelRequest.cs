using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreatePrivateChannelRequest
{
    [JsonProperty("recipient_id")]
    public ulong? RecipientId { get; set; }

    [JsonProperty("recipients")]
    public HashSet<ulong>? Recipients { get; set; }
}
