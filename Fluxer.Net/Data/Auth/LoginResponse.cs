using Newtonsoft.Json;

namespace Fluxer.Net.Data.Auth;

public class LoginResponse : Entity
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}
