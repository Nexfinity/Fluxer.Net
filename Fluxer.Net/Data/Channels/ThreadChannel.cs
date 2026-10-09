namespace Fluxer.Net;

public class ThreadChannel : TextChannel
{
    public ThreadType ThreadType { get; internal set; }

    public bool IsPrivate => ThreadType == ThreadType.PrivateThread;

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
