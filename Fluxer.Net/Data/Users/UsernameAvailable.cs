using Newtonsoft.Json;

namespace Fluxer.Net;

public class UsernameAvailable : Entity
{
    [JsonProperty("taken")]
    public bool Taken { get; set; }
}
