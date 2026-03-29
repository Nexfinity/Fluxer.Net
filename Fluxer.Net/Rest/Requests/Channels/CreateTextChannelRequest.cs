using Newtonsoft.Json;

namespace Fluxer.Net;

public class CreateTextChannelRequest : CreateChannelRequest
{
    public override string Type => "GUILD_TEXT";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
