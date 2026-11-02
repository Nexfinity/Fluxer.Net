namespace Fluxer.Net;

/// <summary>
/// A thread inside a text or news channel.
/// </summary>
public class ThreadChannel : TextChannel, IThreadChannel
{
    /// <inheritdoc />
    public ThreadType ThreadType { get; internal set; }

    /// <inheritdoc />
    public bool IsPrivate => ThreadType == ThreadType.PrivateThread;

    /// <inheritdoc />
    public override ulong? CategoryId => null;

    internal ThreadChannel(FluxerBaseClient client) : base(client)
    {

    }

    internal static ThreadType GetThreadType(ChannelType type)
    {
        switch (type)
        {
            case ChannelType.NewsThread:
                return ThreadType.NewsThread;
            case ChannelType.PublicThread:
                return ThreadType.PublicThread;
            default:
                return ThreadType.PrivateThread;
        }
    }
}
