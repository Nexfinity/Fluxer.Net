using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentApplicationJson : ApplicationJson, IApplication
{
    [JsonProperty("owner")]
    public UserJson Owner { get; set; }
}