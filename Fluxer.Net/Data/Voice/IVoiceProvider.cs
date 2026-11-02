namespace Fluxer.Net;

/// <summary>
/// Joins and leaves voice channels for a client. Fluxer.Net.Voice provides an implementation. Set it on
/// <see cref="FluxerBaseClient.Voice"/> to make <see cref="IVoiceChannel.ConnectAsync"/> work.
/// </summary>
public interface IVoiceProvider
{
    /// <summary>
    /// Joins a voice channel, or moves to it when the bot is already in voice in that guild.
    /// </summary>
    /// <param name="guildId">The guild of the voice channel.</param>
    /// <param name="channelId">The voice channel to join.</param>
    /// <param name="selfDeaf">Whether the bot joins deafened.</param>
    /// <param name="selfMute">Whether the bot joins muted.</param>
    /// <param name="cancellationToken">Cancels the join.</param>
    /// <returns>The open connection.</returns>
    Task<IAudioClient> ConnectAsync(ulong guildId, ulong channelId, bool selfDeaf, bool selfMute,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Joins a voice channel as another connection, leaving the bot's other connections in that guild open. When the
    /// bot is already connected to that channel the open connection is returned.
    /// </summary>
    /// <param name="guildId">The guild of the voice channel.</param>
    /// <param name="channelId">The voice channel to join.</param>
    /// <param name="selfDeaf">Whether the bot joins deafened.</param>
    /// <param name="selfMute">Whether the bot joins muted.</param>
    /// <param name="cancellationToken">Cancels the join.</param>
    /// <returns>The open connection.</returns>
    Task<IAudioClient> ConnectAdditionalAsync(ulong guildId, ulong channelId, bool selfDeaf, bool selfMute,
        CancellationToken cancellationToken = default) =>
        ConnectAsync(guildId, channelId, selfDeaf, selfMute, cancellationToken);

    /// <summary>
    /// Leaves voice in a guild, closing every connection the bot has there.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    Task DisconnectAsync(ulong guildId);

    /// <summary>
    /// Leaves one voice channel, keeping the bot's other connections in that guild.
    /// </summary>
    /// <param name="guildId">The guild of the voice channel.</param>
    /// <param name="channelId">The voice channel to leave.</param>
    Task DisconnectAsync(ulong guildId, ulong channelId) =>
        GetAudioClient(guildId, channelId) is { } connection ? connection.DisconnectAsync() : Task.CompletedTask;

    /// <summary>
    /// Gets an open connection in a guild, or null when the bot is not in voice there. When the bot is in several
    /// channels of the guild, the one joined first is returned.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    IAudioClient? GetAudioClient(ulong guildId);

    /// <summary>
    /// Gets the connection to a voice channel, or null when the bot is not in it.
    /// </summary>
    /// <param name="guildId">The guild of the voice channel.</param>
    /// <param name="channelId">The voice channel.</param>
    IAudioClient? GetAudioClient(ulong guildId, ulong channelId) =>
        GetAudioClient(guildId) is { } connection && connection.ChannelId == channelId ? connection : null;

    /// <summary>
    /// Gets every open connection in a guild.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    IReadOnlyCollection<IAudioClient> GetAudioClients(ulong guildId) =>
        GetAudioClient(guildId) is { } connection ? [connection] : [];
}
