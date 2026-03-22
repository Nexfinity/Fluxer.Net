using Newtonsoft.Json;

namespace Fluxer.Net;

public class UsernameAvailableJson
{
    [JsonProperty("taken")]
    public bool Taken { get; set; }
}
