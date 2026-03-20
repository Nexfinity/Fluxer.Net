using Newtonsoft.Json;

namespace Fluxer.Net;

public class ApplicationJson : PartialApplicationJson, IApplication
{
    /// <inheritdoc />
    [JsonProperty("redirect_urls")]
    public string[] RedirectUrls { get; set; }

    /// <inheritdoc />
    [JsonProperty("bot")]
    public UserJson Bot { get; set; }

    IUser IApplication.Bot => Bot;
}
