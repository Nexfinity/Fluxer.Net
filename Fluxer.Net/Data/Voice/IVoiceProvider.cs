namespace Fluxer.Net;

/// <summary>
/// Joins and leaves voice channels for a client. Fluxer.Net.Voice provides one; set it on
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
    /// Leaves voice in a guild.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    Task DisconnectAsync(ulong guildId);

    /// <summary>
    /// Gets the open connection in a guild, or null when the bot is not in voice there.
    /// </summary>
    /// <param name="guildId">The guild.</param>
    IAudioClient? GetAudioClient(ulong guildId);
}
