using Fluxer.Net.Gateway.Data.Messages;

namespace Fluxer.Net.Commands;

/// <summary>
/// Represents the context of a command execution.
/// </summary>
public class CommandContext
{
    /// <summary>
    /// Gets the API client.
    /// </summary>
    public FluxerClient Client { get; }

    /// <summary>
    /// Gets the API client.
    /// </summary>
    public ApiClient Rest => Client.Rest;

    /// <summary>
    /// Gets the gateway client.
    /// </summary>
    public GatewayClient Gateway => Client.Gateway;

    /// <summary>
    /// Gets the message that triggered the command.
    /// </summary>
    public SocketMessage Message { get; }

    /// <summary>
    /// Gets the channel the command was executed in.
    /// </summary>
    public Channel Channel => Message.Channel;

    /// <summary>
    /// Gets the guild the command was executed in, if any.
    /// </summary>
    public SocketGuild? Guild { get; }

    /// <summary>
    /// Gets the user who executed the command.
    /// </summary>
    public SocketUser User { get; internal set; }

    /// <summary>
    /// Gets the member who executed the command.
    /// </summary>
    public SocketGuildMember? Member { get; }

    /// <summary>
    /// Creates a new command context.
    /// </summary>
    /// <param name="client">The API client.</param>
    /// <param name="message">The message that triggered the command.</param>
    public CommandContext(FluxerClient client, MessageGatewayData message)
    {
        Client = client;
        Message = SocketMessage.Create(client, message);
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
