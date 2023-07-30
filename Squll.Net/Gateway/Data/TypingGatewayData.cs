using Newtonsoft.Json;

namespace Squll.Net.Gateway.Data;

public class TypingGatewayData : IGatewayData
{
    [JsonProperty("space_id")]
    public ulong SpaceId { get; set; }

    [JsonProperty("squad_id")]
    public ulong SquadId { get; set; }
    
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
    
    [JsonProperty("timestamp")]
    public ulong Timestamp { get; set; }
}
