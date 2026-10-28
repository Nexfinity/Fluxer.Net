namespace Fluxer.Net.Voice;

/// <summary>
/// Thrown by <see cref="FluxerVoiceClient.JoinAsync"/> when Fluxer refuses the join, for example because the channel
/// is full.
/// </summary>
public sealed class VoiceJoinRefusedException : Exception
{
    internal VoiceJoinRefusedException(ulong guildId, string code, string? reason)
        : base($"Fluxer refused to join voice in guild {guildId}: {code}{(string.IsNullOrEmpty(reason) ? "" : $" ({reason})")}")
    {
        GuildId = guildId;
        Code = code;
    }

    /// <summary>
    /// The guild of the refused join.
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    /// Fluxer's refusal code, such as <see cref="FluxerErrorCodes.VoiceChannelFull"/>.
    /// </summary>
    public string Code { get; }
}
