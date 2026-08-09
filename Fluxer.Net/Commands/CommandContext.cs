using Fluxer.Net.Gateway;

namespace Fluxer.Net.Commands;

/// <inheritdoc />
public class CommandContext : ICommandContext
{
    /// <inheritdoc/>
    public FluxerClient Client { get; }

    /// <inheritdoc/>
    public ApiClient Rest { get; }

    /// <inheritdoc/>
    public GatewayClient Gateway { get; }

    /// <inheritdoc/>
    public SocketGuild? Server { get; }

    /// <inheritdoc/>
    public Channel Channel { get; }

    /// <inheritdoc/>
    public SocketUser? User { get; }

    /// <inheritdoc/>
    public SocketGuildMember? Member { get; }

    /// <inheritdoc/>
    public SocketMessage Message { get; }

    /// <inheritdoc/>
    public CommandInfo? Command { get; internal set; }

    /// <inheritdoc/>
    public string? Prefix { get; internal set; }

    /// <inheritdoc/>
    public bool IsPrivate => Channel.Type == ChannelType.Dm;

    /// <summary>
    /// Create a new <see cref="CommandContext" /> class with the provided client and message.
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
            Server = client.Gateway.GetGuild(message.GuildId.Value);
            Member = Server.GetMember(User.Id);
            if (Member == null)
            {
                message.Member.User = message.Author;
                Server.AddOrUpdateMember(Client, message.Member);
                Member = Server.GetMember(User.Id);
            }

            if (Member.Server == null)
                Member.Server = Server;
        }
    }
}