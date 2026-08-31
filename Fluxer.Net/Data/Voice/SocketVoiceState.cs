namespace Fluxer.Net;

public class SocketVoiceState : VoiceState
{
    public SocketGuild Server { get; internal set; }
    public SocketVoiceChannel Channel { get; internal set; }

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
        SocketVoiceState data = new SocketVoiceState(client);
        data.Update(client, json);
        data.Server = guild;
        data.Channel = channel;
        return data;
    }

    internal override void Update(FluxerBaseClient client, VoiceStateJson json)
    {
        base.Update(client, json);
    }
}
