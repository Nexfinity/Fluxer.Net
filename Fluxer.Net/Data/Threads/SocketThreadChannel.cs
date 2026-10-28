namespace Fluxer.Net;

public class SocketThreadChannel : SocketTextChannel, IThreadChannel
{
    public ThreadType ThreadType { get; internal set; }

    public bool IsPrivate => ThreadType == ThreadType.PrivateThread;

    public Channel ParentChannel { get; internal set; }

    public override ulong? CategoryId => ParentChannel?.ParentId;

    internal SocketThreadChannel(FluxerBaseClient client) : base(client)
    {

    }
}
