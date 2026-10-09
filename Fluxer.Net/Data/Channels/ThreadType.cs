namespace Fluxer.Net;

/// <summary>
/// The type of thread channel.
/// </summary>
public enum ThreadType
{
    /// <summary>
    /// Thread in a news channel.
    /// </summary>
    NewsThread = 10,

    /// <summary>
    /// Public thread in a text/forum channel.
    /// </summary>
    PublicThread = 11,

    /// <summary>
    /// Private thread in a text/forum channel, only visible to invited users or those with Manage Threads permission.
    /// </summary>
    PrivateThread = 12
}