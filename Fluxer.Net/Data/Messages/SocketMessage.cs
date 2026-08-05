using Fluxer.Net.Gateway;

namespace Fluxer.Net;

public class SocketMessage : Message
{
    public Channel Channel { get; internal set; }

    internal SocketMessage(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketMessage Create(FluxerBaseClient client, MessageGatewayData json)
    {
        var data = new SocketMessage(client);
        data.Channel = (client as FluxerClient).Gateway.GetChannel(json.ChannelId);
        data.Update(client, json);
        return data;
    }
}
