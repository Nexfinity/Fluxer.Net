using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

public class CreateInviteRequest
{
    [JsonProperty("max_uses")]
    public int? MaxUses { get; set; }

    [JsonProperty("max_age")]
    public int? MaxAge { get; set; }

    [JsonProperty("unique")]
    public bool? Unique { get; set; }

    [JsonProperty("temporary")]
    public bool? Temporary { get; set; }
}
