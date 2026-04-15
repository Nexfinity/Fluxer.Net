namespace Fluxer.Net;

public interface IPermissionOverwrite
{
    /// <summary>
    /// The unique identifier for the role or user this overwrite applies to.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    ///  The type of overwrite (0 = role, 1 = member)
    /// </summary>
    int Type { get; }

    /// <summary>
    /// The bitwise value of allowed permissions.
    /// </summary>
    ChannelPermissions Allow { get; }

    /// <summary>
    /// The bitwise value of denied permissions.
    /// </summary>
    ChannelPermissions Deny { get; }
}
