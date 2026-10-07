using Newtonsoft.Json;

namespace Fluxer.Net;

public class FluxerErrorJson
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Reason { get; set; }

    [JsonProperty("errors")]
    FluxerErrorItemJson[]? Errors { get; set; }
}
public class FluxerErrorItemJson
{
    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Reason { get; set; }
}