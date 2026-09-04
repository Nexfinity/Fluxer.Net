using Fluxer.Net.Gateway;

namespace Fluxer.Net;

public class SocketMessage : Message
{
    public Channel Channel { get; private set; }

    public SocketGuild? Guild { get; private set; }

    public new SocketUser Author { get; private set; }

    public SocketGuildMember Member { get; private set; }

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
        SocketMessage data = new SocketMessage(client)
        {
            Channel = (client as FluxerClient).Gateway.GetChannel(json.ChannelId),
            Author = SocketUser.Create(client, json.Author)
        };
        if (json.GuildId.HasValue)
        {
            data.Guild = (client as FluxerClient).Gateway.GetGuild(json.GuildId.Value);
            data.Member = data.Guild.GetMember(json.Author.Id);
            if (data.Member == null)
            {
                json.Member.User = json.Author;
                data.Member = SocketGuildMember.Create(client, json.Member);
                data.Member.Guild = data.Guild;
            }
        }

        data.Update(json);
        return data;
    }
}
