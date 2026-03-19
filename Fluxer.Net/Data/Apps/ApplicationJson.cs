using Newtonsoft.Json;

namespace Fluxer.Net;

public class ApplicationJson : PartialApplicationJson
{
    [JsonProperty("redirect_urls")]
    public string[] RedirectUrls { get; set; }

    [JsonProperty("bot")]
    public UserJson Bot { get; set; }
}
