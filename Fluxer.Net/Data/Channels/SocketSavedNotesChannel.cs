namespace Fluxer.Net;

public class SocketSavedNotesChannel : SavedNotesChannel
{
    internal SocketSavedNotesChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketSavedNotesChannel Create(FluxerBaseClient client, ChannelJson json)
    {
        var data = new SocketSavedNotesChannel(client);
        data.Update(client, json);
        return data;
    }
}
