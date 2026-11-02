namespace Fluxer.Net;

/// <inheritdoc />
public class SocketVoiceState : VoiceState
{
    /// <summary>
    /// The guild of the voice state, or null when it has no guild or the guild is not cached.
    /// </summary>
    public SocketGuild? Guild { get; private set; }

    /// <summary>
    /// The voice channel of the voice state.
    /// </summary>
    public Channel? Channel { get; private set; }

    internal SocketVoiceState(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a voice state from JSON, with its channel and guild.
    /// </summary>
    public static SocketVoiceState Create(FluxerBaseClient client, VoiceStateJson json, Channel channel)
    {
        SocketVoiceState data = new SocketVoiceState(client)
        {
            Channel = channel
        };
        if (json.GuildId.HasValue)
            data.Guild = (client as FluxerClient).Gateway.GetGuild(json.GuildId.Value);

        data.Update(json);
        return data;
    }
}
