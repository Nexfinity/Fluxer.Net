namespace Fluxer.Net;

/// <summary>
/// A call received from the gateway, with its channel.
/// </summary>
public class SocketCall : Call
{
    /// <summary>
    /// The channel the call is in.
    /// </summary>
    public Channel Channel { get; private set; }
    internal SocketCall(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a call from JSON, with its channel.
    /// </summary>
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
