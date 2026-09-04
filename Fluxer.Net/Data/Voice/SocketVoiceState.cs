namespace Fluxer.Net;

/// <inheritdoc />
public class SocketVoiceState : VoiceState
{
    /// <summary>
    /// Guild for the voice state.
    /// </summary>
    public SocketGuild Guild { get; private set; }

    /// <summary>
    /// Channel for the voice state.
    /// </summary>
    public SocketVoiceChannel Channel { get; private set; }

    internal SocketVoiceState(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketVoiceState object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guild"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    public static SocketVoiceState Create(FluxerBaseClient client, VoiceStateJson json, SocketGuild guild, SocketVoiceChannel channel)
    {
        SocketVoiceState data = new SocketVoiceState(client)
        {
            Guild = guild,
            Channel = channel
        };
        data.Update(json);
        return data;
    }
}
