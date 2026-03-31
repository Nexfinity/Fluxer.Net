using Newtonsoft.Json;

namespace Fluxer.Net;

public class CreateTextChannelRequest : CreateGuildChannelRequest
{
    public override string Type => "GUILD_TEXT";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
