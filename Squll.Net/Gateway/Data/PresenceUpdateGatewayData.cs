using Newtonsoft.Json;
using Squll.Net.Objects.Enums;

namespace Squll.Net.Gateway.Data;

public class PresenceUpdateGatewayData : IGatewayData
{
    [JsonProperty("status")]
    public string Status { get; set; }

    // [JsonProperty("activities")]
    // public object[] Activities { get; set; } = Array.Empty<object>();

    public PresenceUpdateGatewayData(Status status)
    {
	    Status = status switch
	    {
		    Objects.Enums.Status.Online => "online",
		    Objects.Enums.Status.Idle => "idle",
		    Objects.Enums.Status.DoNotDisturb => "dnd",
		    Objects.Enums.Status.Invisible => "invisible",
		    _ => Status
	    };
    }
}
