namespace Fluxer.Net;

/// <summary>
/// A thread channel cached from gateway events.
/// </summary>
public class SocketThreadChannel : SocketTextChannel, IThreadChannel
{
    /// <inheritdoc />
    public ThreadType ThreadType { get; internal set; }

    /// <inheritdoc />
    public bool IsPrivate => ThreadType == ThreadType.PrivateThread;

    /// <summary>
    /// The cached parent channel of the thread, or null when it is not cached.
    /// </summary>
    public Channel ParentChannel { get; internal set; }

    /// <inheritdoc />
    public override ulong? CategoryId => ParentChannel?.ParentId;

    internal SocketThreadChannel(FluxerBaseClient client) : base(client)
    {

    }
}
