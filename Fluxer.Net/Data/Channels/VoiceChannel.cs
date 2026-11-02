namespace Fluxer.Net;

/// <summary>
/// A guild voice or stage channel.
/// </summary>
public class VoiceChannel : TextChannel, IVoiceChannel
{
    internal VoiceChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <inheritdoc />
    public Task<IAudioClient> ConnectAsync(bool selfDeaf = true, bool selfMute = false,
        CancellationToken cancellationToken = default) =>
        Client.RequireVoice().ConnectAsync(GuildId!.Value, Id, selfDeaf, selfMute, cancellationToken);

    /// <inheritdoc />
    public Task<IAudioClient> ConnectAdditionalAsync(bool selfDeaf = true, bool selfMute = false,
        CancellationToken cancellationToken = default) =>
        Client.RequireVoice().ConnectAdditionalAsync(GuildId!.Value, Id, selfDeaf, selfMute, cancellationToken);

    /// <inheritdoc />
    public Task DisconnectAsync() => Client.RequireVoice().DisconnectAsync(GuildId!.Value);
}
