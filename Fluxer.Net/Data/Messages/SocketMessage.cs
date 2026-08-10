using Fluxer.Net.Gateway;

namespace Fluxer.Net;

public class SocketMessage : Message
{
    public Channel Channel { get; internal set; }

    internal SocketMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketMessage object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static SocketMessage Create(FluxerBaseClient client, MessageGatewayData json)
    {
        var data = new SocketMessage(client)
        {
            Channel = (client as FluxerClient).Gateway.GetChannel(json.ChannelId)
        };
        data.Update(client, json);
        return data;
    }
}
