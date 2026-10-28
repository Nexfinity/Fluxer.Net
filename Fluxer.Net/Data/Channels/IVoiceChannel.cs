namespace Fluxer.Net;

/// <summary>
/// A guild voice or stage channel.
/// </summary>
public interface IVoiceChannel : ITextChannel, IAudioChannel
{
    /// <summary>
    /// Joins this voice channel, or moves the bot to it when it is already in voice in this guild.
    /// </summary>
    /// <param name="selfDeaf">Whether the bot joins deafened. Bots that only send audio should leave this on.</param>
    /// <param name="selfMute">Whether the bot joins muted.</param>
    /// <param name="cancellationToken">Cancels the join.</param>
    /// <returns>The connection to send audio with.</returns>
    /// <exception cref="InvalidOperationException">Voice is not set up for the client.</exception>
    Task<IAudioClient> ConnectAsync(bool selfDeaf = true, bool selfMute = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Leaves voice in this channel's guild.
    /// </summary>
    /// <exception cref="InvalidOperationException">Voice is not set up for the client.</exception>
    Task DisconnectAsync();
}
