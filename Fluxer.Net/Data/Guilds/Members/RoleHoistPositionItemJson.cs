using Newtonsoft.Json;

namespace Fluxer.Net;

public class RoleHoistPositionItemJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("hoist_position")]
    public int HoistPosition { get; set; }
}
