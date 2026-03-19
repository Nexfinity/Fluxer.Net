using Newtonsoft.Json;

namespace Fluxer.Net;

public class Application : PartialApplication
{
    [JsonProperty("redirect_urls")]
    public string[] RedirectUrls { get; set; }

    [JsonProperty("bot")]
    public User Bot { get; set; }
}
