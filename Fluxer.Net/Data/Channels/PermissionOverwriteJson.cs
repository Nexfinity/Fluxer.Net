using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class PermissionOverwriteJson : IPermissionOverwrite
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("type")]
    public PermissionOverwriteType Type { get; set; }

    /// <inheritdoc />
    [JsonProperty("allow")]
    public ChannelPermissions Allow { get; set; }

    /// <inheritdoc />
    [JsonProperty("deny")]
    public ChannelPermissions Deny { get; set; }
}
/// <summary>
/// The kind of target a permission overwrite applies to.
/// </summary>
public enum PermissionOverwriteType
{
    /// <summary>
    /// The overwrite applies to a role.
    /// </summary>
    Role = 0,

    /// <summary>
    /// The overwrite applies to a member.
    /// </summary>
    Member = 1
}