using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Body for creating or replacing one permission overwrite on a guild channel.
/// </summary>
public class PermissionOverwriteRequest
{
    /// <summary>
    /// Whether the overwrite applies to a role or a member.
    /// </summary>
    [JsonProperty("type")]
    public PermissionOverwriteType Type { get; set; }

    /// <summary>
    /// The permissions to allow, as a bitfield.
    /// </summary>
    [JsonProperty("allow")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Allow { get; set; }

    /// <summary>
    /// The permissions to deny, as a bitfield.
    /// </summary>
    [JsonProperty("deny")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Deny { get; set; }
}
