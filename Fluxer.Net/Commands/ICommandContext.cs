using Fluxer.Net.Gateway;
using Fluxer.Net.Rest;

namespace Fluxer.Net.Commands;

/// <summary>
/// The context of a command: the client, the message and where it was sent.
/// </summary>
public interface ICommandContext
{
    /// <summary>
    /// The command that matched the input.
    /// </summary>
    CommandInfo Command { get; internal set; }

    internal CommandService CommandService { get; set; }

    /// <summary>
    /// The client that received the message.
    /// </summary>
    FluxerClient Client { get; }

    /// <summary>
    /// The REST client of the client.
    /// </summary>
    FluxerApiClient Rest { get; }

    /// <summary>
    /// The gateway client of the client.
    /// </summary>
    FluxerGatewayClient Gateway { get; }

    /// <summary>
    /// The guild the message was sent in, or null outside a guild.
    /// </summary>
    IGuild? Guild { get; }

    /// <summary>
    /// The channel the message was sent in.
    /// </summary>
    IChannel Channel { get; }

    /// <summary>
    /// The author of the message.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// The guild member who sent the message, or null outside a guild.
    /// </summary>
    IGuildMember? Member { get; }

    /// <summary>
    /// The message that triggered the command.
    /// </summary>
    IMessage Message { get; }

    /// <summary>
    /// Whether the message was sent in a direct message or group channel.
    /// </summary>
    bool IsPrivate { get; }
}