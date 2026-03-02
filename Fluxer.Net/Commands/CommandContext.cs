using Fluxer.Net.Gateway.Data;

namespace Fluxer.Net.Commands;

/// <summary>
/// Represents the context of a command execution.
/// </summary>
public class CommandContext
{
    /// <summary>
    /// Gets the API client.
    /// </summary>
    public ApiClient Rest { get; }

    /// <summary>
    /// Gets the gateway client.
    /// </summary>
    public GatewayClient Gateway { get; }

    /// <summary>
    /// Gets the message that triggered the command.
    /// </summary>
    public MessageGatewayData Message { get; }

    /// <summary>
    /// Gets the channel the command was executed in.
    /// </summary>
    public ulong ChannelId => Message.ChannelId;

    /// <summary>
    /// Gets the guild the command was executed in, if any.
    /// </summary>
    public ulong? GuildId => Message.GuildId;

    /// <summary>
    /// Gets the user who executed the command.
    /// </summary>
    public UserPartialResponse User => Message.Author!;

    /// <summary>
    /// Gets the member who executed the command.
    /// </summary>
    public GuildMemberPartialResponse? Member => Message.Member;

    /// <summary>
    /// Creates a new command context.
    /// </summary>
    /// <param name="client">The API client.</param>
    /// <param name="gateway">The gateway client.</param>
    /// <param name="message">The message that triggered the command.</param>
    public CommandContext(FluxerClient client, MessageGatewayData message)
    {
        Rest = client.Rest;
        Gateway = client.Gateway;
        Message = message;
    }
}
