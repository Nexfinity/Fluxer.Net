using Fluxer.Net.Gateway;
using Fluxer.Net.Rest;

namespace Fluxer.Net.Commands;

/// <inheritdoc/>
public class CommandContext : ICommandContext
{
    /// <inheritdoc/>
    public CommandInfo Command { get; internal set; }

    internal CommandService CommandService { get; set; }

    /// <inheritdoc/>
    public FluxerClient Client { get; }

    /// <inheritdoc/>
    public FluxerApiClient Rest { get; }

    /// <inheritdoc/>
    public FluxerGatewayClient Gateway { get; }

    /// <inheritdoc/>
    public SocketGuild Guild { get; }

    /// <inheritdoc/>
    public Channel Channel { get; }

    /// <inheritdoc/>
    public SocketUser User { get; }

    /// <inheritdoc/>
    public SocketGuildMember? Member { get; }

    /// <inheritdoc/>
    public SocketMessage Message { get; }

    /// <inheritdoc/>
    public bool IsPrivate => Channel.Type == ChannelType.Dm;

    IGuild ICommandContext.Guild => Guild;

    IChannel ICommandContext.Channel => Channel;

    IUser ICommandContext.User => User;

    IGuildMember? ICommandContext.Member => Member;

    IMessage ICommandContext.Message => Message;

    CommandService ICommandContext.CommandService { get => CommandService; set => CommandService = value; }
    CommandInfo ICommandContext.Command { get => Command; set => Command = value; }

    /// <summary>
    ///     Initializes a new <see cref="CommandContext" /> class with the provided client and message.
    /// </summary>
    /// <param name="client">The underlying client.</param>
    /// <param name="message">The underlying message.</param>
    public CommandContext(FluxerClient client, MessageGatewayData message)
    {
        Client = client;
        Rest = client.Rest;
        Gateway = client.Gateway;
        Message = SocketMessage.Create(client, message);
        Channel = client.Gateway.GetChannel(message.ChannelId);
        User = SocketUser.Create(client, message.Author);
        if (message.GuildId.HasValue)
        {
            Guild = client.Gateway.GetGuild(message.GuildId.Value);
            Member = Guild.GetMember(User.Id);
            if (Member == null)
            {
                message.Member.User = message.Author;
                Guild.AddOrUpdateMember(Client, message.Member);
                Member = Guild.GetMember(User.Id);
            }

            if (Member.Guild == null)
                Member.Guild = Guild;
        }
    }
}
