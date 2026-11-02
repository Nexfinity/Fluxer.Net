
using Fluxer.Net.Gateway;

namespace Fluxer.Net;

/// <summary>
/// A reaction added to a message, received through the gateway.
/// </summary>
public class SocketReaction : Entity, IReaction
{
    /// <summary>
    /// The ID of the user who reacted.
    /// </summary>
    public ulong UserId { get; private set; }

    /// <summary>
    /// The ID of the message that was reacted to.
    /// </summary>
    public ulong MessageId { get; private set; }

    /// <summary>
    /// The ID of the channel of the message.
    /// </summary>
    public ulong ChannelId { get; private set; }

    /// <summary>
    /// The emoji used for the reaction.
    /// </summary>
    public IEmoji Emoji { get; private set; }

    /// <summary>
    /// The guild member who reacted, or null when it is not known.
    /// </summary>
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