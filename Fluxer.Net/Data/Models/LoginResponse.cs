using Newtonsoft.Json;

namespace Fluxer.Net.Objects.Models;

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}
