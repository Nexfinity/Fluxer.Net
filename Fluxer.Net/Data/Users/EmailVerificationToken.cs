using Newtonsoft.Json;

namespace Fluxer.Net;

public class EmailVerificationToken : Entity
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}
