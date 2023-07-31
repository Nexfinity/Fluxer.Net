using Newtonsoft.Json;
using Squll.Net.Objects;

namespace Squll.Net.Gateway.Data;

public class IdentifyGatewayData : IGatewayData
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("properties")]
    public GatewayConnectionProperties Properties { get; set; } = new();

    public IdentifyGatewayData(string token)
        => Token = token;
}
