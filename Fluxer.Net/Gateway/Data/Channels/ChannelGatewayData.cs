using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway channel data matching the ChannelResponse API model
/// </summary>
public class ChannelGatewayData : ChannelJson
{
    [JsonProperty("recipients")]
    public List<UserJson>? Recipients { get; set; }
}

/// <summary>
/// Channel permission overwrite response
/// </summary>
public class ChannelOverwriteResponse
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("allow")]
    public string Allow { get; set; } = null!;

    [JsonProperty("deny")]
    public string Deny { get; set; } = null!;
}
