using Fluxer.Net.Extensions;
using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// A permission overwrite sent when creating or updating a guild channel.
/// </summary>
public class ChannelOverwriteRequest
{
    /// <summary>
    /// The ID of the role or member the overwrite applies to.
    /// </summary>
    [JsonProperty("id")]
    [JsonConverter(typeof(StringUInt64Converter))]
    public ulong Id { get; set; }

    /// <summary>
    /// Whether the overwrite applies to a role or a member.
    /// </summary>
    [JsonProperty("type")]
    public OverwriteRequestType Type { get; set; }

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
