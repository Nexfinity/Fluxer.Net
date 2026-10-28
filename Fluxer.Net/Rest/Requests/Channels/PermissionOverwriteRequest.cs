using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Body for creating or replacing one permission overwrite on a guild channel.
/// </summary>
public class PermissionOverwriteRequest
{
    [JsonProperty("type")]
    public PermissionOverwriteType Type { get; set; }

    [JsonProperty("allow")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Allow { get; set; }

    [JsonProperty("deny")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Deny { get; set; }
}
