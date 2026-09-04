using Fluxer.Net.Gateway;
using Fluxer.Net.Rest;

namespace Fluxer.Net.Commands;

/// <summary>
/// The context of a command. This may include the client, guild, channel, user, and message.
/// </summary>
public interface ICommandContext
{
    /// <summary>
    /// Command info from the command used.
    /// </summary>
    CommandInfo Command { get; internal set; }

    internal CommandService CommandService { get; set; }

    /// <summary>
    /// Fluxer base client for the context.
    /// </summary>
    FluxerClient Client { get; }

    /// <summary>
    /// Fluxer http client for the context.
    /// </summary>
    FluxerApiClient Rest { get; }

    /// <summary>
    /// Fluxer gateway client for the context.
    /// </summary>
    FluxerGatewayClient Gateway { get; }

    /// <summary>
    /// Current guild for the context.
    /// </summary>
    IGuild? Guild { get; }

    /// <summary>
    /// Current channel for the context.
    /// </summary>
    IChannel Channel { get; }

    /// <summary>
    /// Current user for the context.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// Current server member for the context.
    /// </summary>
    IGuildMember? Member { get; }

    /// <summary>
    /// Current message for the context.
    /// </summary>
    IMessage Message { get; }

    /// <summary>
    /// Channel is a DM.
    /// </summary>
    bool IsPrivate { get; }
}