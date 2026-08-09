namespace Fluxer.Net.Commands;

/// <summary>
/// The context of a command which may contain the client, user, guild, channel, and message.
/// </summary>
public interface ICommandContext
{
    /// <summary>
    /// Fluxer base client for the context.
    /// </summary>
    FluxerClient Client { get; }

    /// <summary>
    /// Fluxer http client for the context.
    /// </summary>
    ApiClient Rest { get; }

    /// <summary>
    /// Fluxer gateway client for the context.
    /// </summary>
    GatewayClient Gateway { get; }

    /// <summary>
    /// Current server for the context.
    /// </summary>
    SocketGuild? Server { get; }

    /// <summary>
    /// Current channel for the context.
    /// </summary>
    Channel Channel { get; }

    /// <summary>
    /// Current user for the context.
    /// </summary>
    SocketUser User { get; }

    /// <summary>
    /// Current server member for the context.
    /// </summary>
    SocketGuildMember? Member { get; }

    /// <summary>
    /// Current message for the context.
    /// </summary>
    SocketMessage Message { get; }

    /// <summary>
    /// Command that has been run.
    /// </summary>
    CommandInfo? Command { get; }

    /// <summary>
    /// Prefix used for the command.
    /// </summary>
    string? Prefix { get; }

    /// <summary>
    /// Channel is a DM.
    /// </summary>
    bool IsPrivate { get; }
}
