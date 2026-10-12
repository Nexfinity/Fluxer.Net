namespace Fluxer.Net;

public class SocketCall : Call
{
    public Channel Channel { get; private set; }
    internal SocketCall(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketCall object from json.
    /// </summary>
    /// <returns><see cref="SocketCall"/></returns>
    public static SocketCall Create(FluxerBaseClient client, CallJson json, Channel channel)
    {
        SocketCall data = new SocketCall(client)
        {
            Channel = channel,
        };
        data.Update(json);
        return data;
    }
}
