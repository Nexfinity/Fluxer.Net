using Fluxer.Net.Data.Enums;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class ChannelOverwriteRequest
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public ChannelOverwriteRequestType Type { get; set; }

    [JsonPropertyName("allow")]
    public ulong Allow { get; set; }

    [JsonPropertyName("deny")]
    public ulong Deny { get; set; }
}
