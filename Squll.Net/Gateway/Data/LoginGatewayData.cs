using Newtonsoft.Json;

namespace Squll.Net.Gateway;

public class LoginGatewayData : IGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    public LoginGatewayData(string token)
        => Token = token;
}
