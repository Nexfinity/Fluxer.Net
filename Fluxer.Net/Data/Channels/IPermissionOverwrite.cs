namespace Fluxer.Net;

/// <summary>
/// A permission overwrite for a role or member in a channel.
/// </summary>
public interface IPermissionOverwrite
{
    /// <summary>
    /// The ID of the role or member the overwrite applies to.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// The kind of target the overwrite applies to.
    /// </summary>
    PermissionOverwriteType Type { get; }

    /// <summary>
    /// The permissions the overwrite allows.
    /// </summary>
    ChannelPermissions Allow { get; }

    /// <summary>
    /// The permissions the overwrite denies.
    /// </summary>
    ChannelPermissions Deny { get; }
}
