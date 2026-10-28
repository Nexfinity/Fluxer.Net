namespace Fluxer.Net.Voice;

/// <summary>
/// Thrown by <see cref="FluxerVoiceClient.JoinAsync"/> when Fluxer does not send a voice server in time.
/// </summary>
public sealed class VoiceJoinTimeoutException : TimeoutException
{
    internal VoiceJoinTimeoutException(ulong guildId)
        : base($"Fluxer did not send a voice server for guild {guildId} in time.")
    {
        GuildId = guildId;
    }

    /// <summary>
    /// The guild of the join that timed out.
    /// </summary>
    public ulong GuildId { get; }
}
