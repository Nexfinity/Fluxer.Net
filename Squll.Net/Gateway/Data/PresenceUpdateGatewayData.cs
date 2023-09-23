using Newtonsoft.Json;
using Squll.Net.Objects.Data;
using StatusEnum = Squll.Net.Objects.Data.Status;

namespace Squll.Net.Gateway.Data;

public class PresenceUpdateGatewayData : IGatewayData
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
			StatusEnum.DoNotDisturb => "dnd",
			StatusEnum.Invisible => "invisible",
			_ => Status
		};
	}
}
