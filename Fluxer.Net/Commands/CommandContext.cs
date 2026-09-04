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
    public SocketGuild? Guild { get; }

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
    public CommandContext(FluxerClient client, SocketMessage message)
    {
        Client = client;
        Rest = client.Rest;
        Gateway = client.Gateway;
        Message = message;
        Channel = client.Gateway.GetChannel(message.ChannelId);
        User = message.Author;
        if (message.Channel.GuildId.HasValue)
        {
            Guild = message.Guild;
            Member = message.Member;
        }
    }
}
