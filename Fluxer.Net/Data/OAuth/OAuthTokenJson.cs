using Newtonsoft.Json;

namespace Fluxer.Net;

public class OAuthTokenJson
{
    [JsonProperty("application")]
    public PartialApplicationJson Application { get; set; }

    [JsonProperty("scopes")]
    public string[] Scopes { get; set; }

    [JsonProperty("expires")]
    public DateTime Expires { get; set; }

    [JsonProperty("user")]
    public UserJson User { get; set; }
}
