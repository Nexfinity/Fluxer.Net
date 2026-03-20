using Newtonsoft.Json;

namespace Fluxer.Net;

public class InviteChannelJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }
}
