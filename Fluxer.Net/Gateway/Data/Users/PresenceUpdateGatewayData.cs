using Newtonsoft.Json;
using StatusEnum = Fluxer.Net.Status;

namespace Fluxer.Net.Gateway.Data.Users;

public class PresenceUpdateGatewayData
{
    [JsonProperty("status")]
    public string Status { get; set; }

    // [JsonProperty("activities")]
    // public object[] Activities { get; set; } = Array.Empty<object>();

    public PresenceUpdateGatewayData(StatusEnum status)
    {
        Status = status switch
        {
            StatusEnum.Online => "online",
            StatusEnum.Idle => "idle",
            StatusEnum.Dnd => "dnd",
            StatusEnum.Invisible => "invisible",
            _ => Status
        };
    }
}
