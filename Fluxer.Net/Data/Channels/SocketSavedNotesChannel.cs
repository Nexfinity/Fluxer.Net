namespace Fluxer.Net;

/// <summary>
/// A cached personal notes channel for the current user.
/// </summary>
public class SocketSavedNotesChannel : SavedNotesChannel
{
    internal SocketSavedNotesChannel(FluxerBaseClient client) : base(client)
    {

    }

}
