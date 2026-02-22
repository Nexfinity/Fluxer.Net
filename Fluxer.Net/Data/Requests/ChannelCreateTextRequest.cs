using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

public class ChannelCreateTextRequest : ChannelCreateRequest
{
    public override string Type => "GUILD_TEXT";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
