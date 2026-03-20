using Newtonsoft.Json;

namespace Fluxer.Net;

public class InviteJson : PartialInviteJson
{
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("uses")]
    public int Uses { get; set; }

    [JsonProperty("max_uses")]
    public int MaxUses { get; set; }

    [JsonProperty("max_age")]
    public int MaxAge { get; set; }
}
