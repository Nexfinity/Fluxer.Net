using Newtonsoft.Json;

namespace Fluxer.Net;

public class OAuthToken : Entity
{
    [JsonProperty("application")]
    public PartialApplication Application { get; set; }

    [JsonProperty("scopes")]
    public string[] Scopes { get; set; }

    [JsonProperty("expires")]
    public DateTime Expires { get; set; }

    [JsonProperty("user")]
    public User User { get; set; }
}
