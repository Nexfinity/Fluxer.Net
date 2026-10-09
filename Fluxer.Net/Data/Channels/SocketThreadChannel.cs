namespace Fluxer.Net;

public class SocketThreadChannel : SocketTextChannel
{
    public ThreadType ThreadType { get; internal set; }

    public bool IsPrivate => ThreadType == ThreadType.PrivateThread;

    internal SocketThreadChannel(FluxerBaseClient client) : base(client)
    {

    }
}
