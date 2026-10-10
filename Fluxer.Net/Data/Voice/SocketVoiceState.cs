namespace Fluxer.Net;

/// <inheritdoc />
public class SocketVoiceState : VoiceState
{
    /// <summary>
    /// Guild for the voice state.
    /// </summary>
    public SocketGuild? Guild { get; private set; }

    /// <summary>
    /// Channel for the voice state.
    /// </summary>
    public Channel? Channel { get; private set; }

    internal SocketVoiceState(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketVoiceState object from json.
    /// </summary>
    /// <returns><see cref="SocketVoiceState"/></returns>
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
