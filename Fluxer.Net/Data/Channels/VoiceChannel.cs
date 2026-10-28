namespace Fluxer.Net;

public class VoiceChannel : TextChannel, IVoiceChannel
{
    internal VoiceChannel(FluxerBaseClient client) : base(client)
    {

    }
}
