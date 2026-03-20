using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class LoginJson : ILogin
{
    /// <inheritdoc />
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}
