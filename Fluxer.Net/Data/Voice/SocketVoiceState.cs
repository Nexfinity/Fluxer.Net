namespace Fluxer.Net;

public class SocketVoiceState : VoiceState
{
    public SocketGuild Guild { get; internal set; }
    public SocketVoiceChannel? Channel { get; internal set; }

    internal SocketVoiceState(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketVoiceState Create(FluxerBaseClient client, VoiceStateJson json, SocketGuild guild)
    {
        var data = new SocketVoiceState(client);
        data.Update(client, json);
        data.Guild = guild;
        return data;
    }

    internal override void Update(FluxerBaseClient client, VoiceStateJson json)
    {
        base.Update(client, json);
        Channel = json.ChannelId.HasValue ? (SocketVoiceChannel)(client as FluxerClient).Gateway.GetChannel(json.ChannelId.Value) : null;
    }
}
