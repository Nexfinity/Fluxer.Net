
using Fluxer.Net.Gateway;

namespace Fluxer.Net;

public class SocketReaction : Entity, IReaction
{
    public ulong UserId { get; private set; }

    public ulong MessageId { get; private set; }

    public ulong ChannelId { get; private set; }

    public IEmoji Emoji { get; private set; }

    public SocketGuildMember? Member { get; private set; }

    internal SocketReaction(FluxerBaseClient client) : base(client)
    {

    }

    internal static SocketReaction Create(FluxerBaseClient client, MessageReactionGatewayData json, SocketGuildMember? member)
    {
        SocketReaction data = new SocketReaction(client)
        {
            Member = member
        };
        data.Update(json);
        return data;
    }

    internal void Update(MessageReactionGatewayData json)
    {
        UserId = json.UserId;
        MessageId = json.MessageId;
        ChannelId = json.ChannelId;
        Emoji = GuildEmoji.Create(Client, json.Emoji);
    }
}